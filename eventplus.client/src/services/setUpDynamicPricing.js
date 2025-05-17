import { dynamicPricingService } from './dynamicPricingService';

/**
 * Initialize the dynamic pricing scheduler
 * This should be called when the application starts
 */
export const initializeDynamicPricing = () => {
  try {
    console.log('Initializing dynamic pricing system...');
    
    // Start the scheduler that runs every 24 hours
    dynamicPricingService.startPriceAdjustmentScheduler();
    
    console.log('Dynamic pricing system initialized successfully.');
  } catch (error) {
    console.error('Failed to initialize dynamic pricing system:', error);
  }
};

export default initializeDynamicPricing;