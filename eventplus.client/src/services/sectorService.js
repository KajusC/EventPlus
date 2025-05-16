import apiClient from './apiClient';

const API_ENDPOINT = '/Sector';

export const fetchSectors = async () => {
  const response = await apiClient.get(API_ENDPOINT);
  return response.data;
};

export const fetchSectorById = async (id) => {
  const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
  return response.data;
};