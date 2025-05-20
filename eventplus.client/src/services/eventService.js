import apiClient from './apiClient';

const API_ENDPOINT = '/Event';

export const fetchEvents = async () => {
  const response = await apiClient.get(API_ENDPOINT);
  return response.data;
};

export const fetchEventById = async (id) => {
  const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
  return response.data;
};

export const fetchRecommendedEvents = async () => {
  try {
    console.log('Fetching recommended events (rating 7/10 or higher)');
    const response = await apiClient.get(`${API_ENDPOINT}/toprated`);
    return response.data;
  } catch (error) {
    console.error('Error fetching recommended events:', error);
    // If server returns 404, it means no highly rated events were found
    if (error.response?.status === 404) {
      throw { response: { status: 404, data: 'No highly rated events found' } };
    }
    throw error;
  }
};

export const createEvent = async (eventData) => {
  // Klonuojame duomenis, kad nepakeistume originalių
  const processedData = JSON.parse(JSON.stringify(eventData));
  
  if (processedData.event) {
    // Jei fkEventLocationidEventLocation yra null arba undefined, nustatome į 0
    if (processedData.event.fkEventLocationidEventLocation === null || 
        processedData.event.fkEventLocationidEventLocation === undefined) {
      processedData.event.fkEventLocationidEventLocation = 0;
    }
  }
  // Tikrinama, ar yra sektoriai
  if (processedData.sectors && processedData.sectors.length > 0) {
    // Sukuriame sektoriaus ID -> indekso žemėlapį
    const sectorIdToIndexMap = {};
    
    // Įsitikiname, kad sektoriai turi teisingą ryšį su lokacija
    processedData.sectors.forEach((sector, index) => {
      // Išsaugome tempId -> indeksas
      if (sector.tempId) {
        sectorIdToIndexMap[sector.tempId] = index;
      }
      
      // Tikrai užtikriname, kad sektorius turi teisingą lokacijos ID
      if (processedData.eventLocation?.idEventLocation > 0) {
        sector.fkEventLocationidEventLocation = processedData.eventLocation.idEventLocation;
      }
      
      // Išvalome tempId, nes jų serveris nesupras
      delete sector.tempId;
    });
    
    // Tvarkome kainas (SectorPrices)
    if (processedData.sectorPrices && processedData.sectorPrices.length > 0) {
      processedData.sectorPrices = processedData.sectorPrices.map(price => {
        const newPrice = { ...price };
        
        // Jei sectorId yra string (tempId), konvertuojame į indeksą
        if (typeof price.sectorId === 'string' && sectorIdToIndexMap[price.sectorId] !== undefined) {
          newPrice.sectorId = sectorIdToIndexMap[price.sectorId];
        }
        
        // Išvalome tempId
        delete newPrice.tempId;
        return newPrice;
      });
    }
    
    // Tvarkome sėdimas vietas (Seatings)
    if (processedData.seatings && processedData.seatings.length > 0) {
      processedData.seatings = processedData.seatings.map(seat => {
        const newSeat = { ...seat };
        
        // Jei sectorId yra string (tempId), konvertuojame į indeksą
        if (typeof seat.sectorId === 'string' && sectorIdToIndexMap[seat.sectorId] !== undefined) {
          newSeat.sectorId = sectorIdToIndexMap[seat.sectorId];
        }
        
        // Išvalome tempId
        delete newSeat.tempId;
        return newSeat;
      });
    }
  }
  
 // Tvarkome eventLocation objektą
  if (processedData.eventLocation) {
    // Pašaliname sectorIds, nes jie nereikalingi kuriant naują vietą
    if (processedData.eventLocation.sectorIds) {
      delete processedData.eventLocation.sectorIds;
    }
    
    
    // Įsitikinome, kad idEventLocation yra 0, kad būtų sukurta nauja vieta
    processedData.eventLocation.idEventLocation = 0;
  }
  
   if (processedData.event && processedData.event.fkEventLocationidEventLocation === null) {
    processedData.event.fkEventLocationidEventLocation = 0;
  }
  
  console.log("Submitting processed data:", JSON.stringify(processedData, null, 2));
  
  try {
    const response = await apiClient.post(`${API_ENDPOINT}/CreateFullEvent`, processedData);
    return response.data;
  } catch (error) {
    console.error("Error creating event:", error);
    
    if (error.response) {
      console.error("Server response:", error.response.data);
      
      // Detalesnė klaidos informacija
      let errorMessage = "Failed to create event. Server returned an error.";
      
      if (error.response.data && error.response.data.errors) {
        // Sujungti validacijos klaidas į vieną pranešimą
        const errorDetails = Object.entries(error.response.data.errors)
          .map(([field, errors]) => `${field}: ${errors.join(', ')}`)
          .join(' | ');
        
        errorMessage = `Validation errors: ${errorDetails}`;
      } else if (error.response.data && error.response.data.title) {
        errorMessage = error.response.data.title;
      } else if (typeof error.response.data === 'string') {
        errorMessage = error.response.data;
      }
      
      throw new Error(errorMessage);
    }
    
    throw error;
  }
};

export const updateEvent = async (id, eventData) => {
  const response = await apiClient.put(API_ENDPOINT, eventData);
  return response.data;
};

export const deleteEvent = async (id) => {
  const response = await apiClient.delete(`${API_ENDPOINT}/${id}`);
  return response.data;
};
export const fetchVenueSuitability = async (eventFormData) => {
  try {
    const response = await apiClient.post('/Event/venue-suitability', eventFormData);
    return response.data;
  } catch (error) {
    console.error('Error fetching venue suitability:', error.response?.data || error.message);
    throw error.response?.data || error;
  }
};