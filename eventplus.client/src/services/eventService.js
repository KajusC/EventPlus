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

  const processedData = JSON.parse(JSON.stringify(eventData));
  
  const sectorIndexMap = {};
  
  if (processedData.sectors && processedData.sectors.length > 0) {
    processedData.sectors.forEach((sector, index) => {
      if (sector.tempId) {
        sectorIndexMap[sector.tempId] = index;
        
        delete sector.tempId;
      }
    });
    
    if (processedData.sectorPrices && processedData.sectorPrices.length > 0) {
      processedData.sectorPrices.forEach(price => {
        if (typeof price.sectorId === 'string' && price.sectorId.startsWith('temp-')) {
          const sectorIndex = sectorIndexMap[price.sectorId];
          if (sectorIndex !== undefined) {
            price.sectorId = sectorIndex;
          } else {
            console.warn(`Could not find mapping for sector tempId: ${price.sectorId}`);
          }
        }
      });
    }

    if (processedData.seatings && processedData.seatings.length > 0) {
      processedData.seatings.forEach(seat => {
        if (typeof seat.sectorId === 'string' && seat.sectorId.startsWith('temp-')) {
          const sectorIndex = sectorIndexMap[seat.sectorId];
          if (sectorIndex !== undefined) {
            seat.sectorId = sectorIndex;
          } else {
            console.warn(`Could not find mapping for sector tempId: ${seat.sectorId}`);
          }
        }
      });
    }
    
    if (processedData.eventLocation) {
      processedData.eventLocation.sectorIds = [0];
    }
  }
  
  console.log("Submitting processed data:", JSON.stringify(processedData, null, 2));
  
  try {
    const response = await apiClient.post(`${API_ENDPOINT}/CreateFullEvent`, processedData);
    return response.data;
  } catch (error) {
    // Enhanced error handling
    console.error("Error creating event:", error);
    if (error.response) {
      console.error("Server response:", error.response.data);
      throw new Error(typeof error.response.data === 'string' ? error.response.data : "Failed to create event. Server returned an error.");
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