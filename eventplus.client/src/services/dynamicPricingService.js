import { fetchEvents, fetchEventById, updateEvent } from './eventService';
import { fetchTickets } from './ticketService';
import { fetchSectorPrices, updateSectorPrice } from './sectorPriceService';
import { fetchOrganiserById } from './authService';

// Keep track of whether the scheduler is running to prevent duplicate initialization
let schedulerRunning = false;

export const dynamicPricingService = {
  // Schedule the price adjustment to run every 24 hours
  startPriceAdjustmentScheduler: (runImmediately = false) => {
    if (schedulerRunning) {
      console.log('Dynamic pricing scheduler is already running');
      return;
    }
    
    console.log('Starting dynamic pricing scheduler');
    schedulerRunning = true;
    
    // Only run immediately if specifically requested
    if (runImmediately) {
      dynamicPricingService.adjustAllEventPrices();
    }
    
    const ONE_DAY_MS = 24 * 60 * 60 * 1000; // 10 * 1000 (for teststing) // 24 * 60 * 60 * 1000
    setInterval(() => {
      dynamicPricingService.adjustAllEventPrices();
    }, ONE_DAY_MS);
  },

  adjustAllEventPrices: async () => {
    try {
      console.log('Running price adjustment for all events...');
      const events = await fetchEvents();
      
      const activeEvents = events.filter(event => {
        return new Date(event.endDate) > new Date();
      });
      
      await Promise.all(activeEvents.map(async (event) => {
        await dynamicPricingService.adjustEventPrice(event.idEvent);
      }));
      
      console.log('Price adjustment complete for all events');
    } catch (error) {
      console.error('Error adjusting prices:', error);
    }
  },

  adjustEventPrice: async (eventId) => {
    try {
      const event = await fetchEventById(eventId);
      if (!event) {
        console.error(`Event with ID ${eventId} not found`);
        return;
      }
      
      const allSectorPrices = await fetchSectorPrices();
      const eventSectorPrices = allSectorPrices.filter(sectorPrice => sectorPrice.eventId === eventId);
      
      const allTickets = await fetchTickets();
      const eventTickets = allTickets.filter(ticket => ticket.fkEventidEvent === eventId);
      
      const organiser = await fetchOrganiserById(event.fkOrganiseridUser);
      
      let marketabilityFactor = 1.0;
      
      const [
        priceAnalysisResult,
        salesAnalysisResult,
        categoryAnalysisResult,
        organiserAnalysisResult
      ] = await Promise.all([
        dynamicPricingService.analyzePrices(eventSectorPrices),
        dynamicPricingService.analyzeSalesPerformance(event, eventTickets),
        dynamicPricingService.analyzeCategoryPrices(event),
        dynamicPricingService.analyzeOrganiserFactors(organiser)
      ]);
      
      marketabilityFactor += priceAnalysisResult.marketabilityAdjustment;
      marketabilityFactor += salesAnalysisResult.marketabilityAdjustment;
      marketabilityFactor += categoryAnalysisResult.marketabilityAdjustment;
      marketabilityFactor += organiserAnalysisResult.marketabilityAdjustment;
      
      await dynamicPricingService.updateSectorPrices(event, eventSectorPrices, marketabilityFactor);
      
      console.log(`Adjusted prices for event ${eventId} with marketability factor: ${marketabilityFactor}`);
      return { eventId, marketabilityFactor };
    } catch (error) {
      console.error(`Error adjusting price for event ${eventId}:`, error);
      return { eventId, error: error.message };
    }
  },

  analyzePrices: async (eventSectorPrices) => {
    return { marketabilityAdjustment: 0 };
  },

  // Process 2: Analyze sales performance and time factors
  analyzeSalesPerformance: async (event, eventTickets) => {
    let marketabilityAdjustment = 0;
    
    const totalTicketCount = event.maxTicketCount || 0;
    
    // Calculate sold tickets
    const soldTickets = eventTickets.length;
    const soldPercentage = totalTicketCount > 0 ? (soldTickets / totalTicketCount) * 100 : 0;
    const unsoldPercentage = 100 - soldPercentage;
    
    const oneMonthAgo = new Date();
    oneMonthAgo.setMonth(oneMonthAgo.getMonth() - 1);
    
    const recentlySoldTickets = eventTickets.filter(ticket => {
      const ticketDate = new Date(ticket.generationDate);
      return ticketDate >= oneMonthAgo;
    });
    
    const monthlySalePercentage = totalTicketCount > 0 
      ? (recentlySoldTickets.length / totalTicketCount) * 100 
      : 0;
    
    if (monthlySalePercentage < 1) {
      marketabilityAdjustment -= 0.1;
    } else if (monthlySalePercentage > 10) {
      marketabilityAdjustment += 0.1;
    }
    
    if (unsoldPercentage > 90) {
      marketabilityAdjustment -= 0.1;
    } else if (unsoldPercentage < 20) {
      marketabilityAdjustment += 0.1;
    }
    
    const currentDate = new Date();
    const eventStartDate = new Date(event.startDate);
    const monthsUntilEvent = (eventStartDate - currentDate) / (30 * 24 * 60 * 60 * 1000);
    
    if (monthsUntilEvent >= 6) {
      marketabilityAdjustment -= 0.1;
    } else if (monthsUntilEvent < 1) {
      marketabilityAdjustment += 0.1;
    }
    
    return { marketabilityAdjustment };
  },

  // Process 3: Analyze category price trends
  analyzeCategoryPrices: async (event) => {
    let marketabilityAdjustment = 0;
    
    try {
      const allEvents = await fetchEvents();
      const sameCategory = allEvents.filter(e => e.category === event.category && e.idEvent !== event.idEvent);
      
      if (sameCategory.length === 0) {
        return { marketabilityAdjustment: 0 };
      }
      
      const allSectorPrices = await fetchSectorPrices();
      
      const sameCategoryEventIds = sameCategory.map(e => e.idEvent);
      const sameCategorySectorPrices = allSectorPrices.filter(sectorPrice => 
        sameCategoryEventIds.includes(sectorPrice.eventId)
      );
      
      const sectorPriceValues = sameCategorySectorPrices.map(sectorPrice => sectorPrice.price).filter(price => price > 0);
      
      if (sectorPriceValues.length === 0) {
        return { marketabilityAdjustment: 0 };
      }
      
      const priceFrequency = {};
      let maxFrequency = 0;
      let mode = 0;
      
      sectorPriceValues.forEach(price => {
        const roundedPrice = Math.round(price);
        if (!priceFrequency[roundedPrice]) {
          priceFrequency[roundedPrice] = 0;
        }
        priceFrequency[roundedPrice]++;
        
        if (priceFrequency[roundedPrice] > maxFrequency) {
          maxFrequency = priceFrequency[roundedPrice];
          mode = roundedPrice;
        }
      });
      
      marketabilityAdjustment = mode * 0.001;
      
      return { marketabilityAdjustment };
    } catch (error) {
      console.error('Error analyzing category prices:', error);
      return { marketabilityAdjustment: 0 };
    }
  },

  // Process 4: Analyze organiser factors
  analyzeOrganiserFactors: async (organiser) => {
    let marketabilityAdjustment = 0;
    
    if (!organiser) {
      return { marketabilityAdjustment };
    }
    
    const followerCount = organiser.followerCount || 0;
    
    if (followerCount < 1000) {
      marketabilityAdjustment -= 0.1;
    } else if (followerCount > 100000) {
      marketabilityAdjustment += 0.1;
    }
    
    const rating = organiser.rating || 0;
    
    if (rating < 3) {
      marketabilityAdjustment -= 0.1;
    } else if (rating > 8.5) {
      marketabilityAdjustment += 0.1;
    }
    
    return { marketabilityAdjustment };
  },

  updateSectorPrices: async (event, eventSectorPrices, marketabilityFactor) => {
    try {
      if (!event || !eventSectorPrices || eventSectorPrices.length === 0) return;
      
      for (const sectorPrice of eventSectorPrices) {
        const newPrice = sectorPrice.price * marketabilityFactor;
        
        const updatedSectorPrice = {
          ...sectorPrice,
          price: newPrice
        };
        console.log(newPrice);
        console.log("AI");
        await updateSectorPrice(updatedSectorPrice);
      }
      
      console.log(`Updated sector prices for event ${event.idEvent}`);
    } catch (error) {
      console.error(`Error updating sector prices for event ${event.idEvent}:`, error);
    }
  },

  triggerPriceAdjustment: async (eventId) => {
    return await dynamicPricingService.adjustEventPrice(eventId);
  }
};

export default dynamicPricingService;