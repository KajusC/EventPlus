import { dynamicPricingService } from './dynamicPricingService';

export const initializeDynamicPricing = (runImmediately = false) => {
  try {
    console.log('Initializing dynamic pricing system...');
    
    // Start the scheduler that runs every 24 hours
    // Pass the runImmediately parameter to control immediate execution
    dynamicPricingService.startPriceAdjustmentScheduler(runImmediately);
    
    console.log('Dynamic pricing system initialized successfully.');
  } catch (error) {
    console.error('Failed to initialize dynamic pricing system:', error);
  }
};

export default initializeDynamicPricing;