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

export const createEvent = async (eventData) => {
  const response = await apiClient.post(`${API_ENDPOINT}/CreateFullEvent`, eventData);
  return response.data;
};

export const updateEvent = async (id, eventData) => {
  const response = await apiClient.put(API_ENDPOINT, eventData);
  return response.data;
};

export const deleteEvent = async (id) => {
  const response = await apiClient.delete(`${API_ENDPOINT}/${id}`);
  return response.data;
};