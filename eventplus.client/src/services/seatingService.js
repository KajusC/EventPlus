import apiClient from './apiClient';

const API_ENDPOINT = '/Seating';

export const fetchSeatings = async () => {
  const response = await apiClient.get(API_ENDPOINT);
  return response.data;
};

export const fetchSeatingById = async (id) => {
  const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
  return response.data;
};