import apiClient from './apiClient';

const API_ENDPOINT = '/SectorPrice';

export const fetchSectorPrices = async () => {
  const response = await apiClient.get(API_ENDPOINT);
  return response.data;
};

export const fetchSectorPriceById = async (id) => {
  const response = await apiClient.get(`${API_ENDPOINT}/${id},${eventId}`);
  return response.data;
};