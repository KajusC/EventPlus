import apiClient from './apiClient';

const ENDPOINT = '/EventLocation';

export const getEventLocations = async () => {
  const response = await apiClient.get(ENDPOINT);
  return response.data;
};

export const getEventLocationById = async (id) => {
  const response = await apiClient.get(`${ENDPOINT}/${id}`);
  return response.data;
};

export const getFeedbackByLocation = async (locationId) => {
  const response = await apiClient.get(`/Feedback/location/${locationId}`);
  return response.data;
};

export const getLocationRatings = async (locationId) => {
  const response = await apiClient.get(`${ENDPOINT}/${locationId}/ratings`);
  return response.data;
};