import apiClient from './apiClient';

const API_ENDPOINT = '/Ticket';

export const fetchTickets = async () => {
  const response = await apiClient.get(API_ENDPOINT);
  return response.data;
};

export const fetchTicketById = async (id) => {
  const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
  return response.data;
};

export const createTicket = async (ticketData) => {
  const response = await apiClient.post(API_ENDPOINT, ticketData);
  return response.data;
};

export const updateTicket = async (ticketData) => {
  const response = await apiClient.put(API_ENDPOINT, ticketData);
  return response.data;
};

export const deleteTicket = async (id) => {
  const response = await apiClient.delete(`${API_ENDPOINT}/${id}`);
  return response.data;
};

export const downloadTicketPdf = async (ticketId) => {
    try {
        const response = await apiClient.get(`${API_ENDPOINT}/generatePdf/${ticketId}`, {
            responseType: 'blob',
        });
        return response.data;
    } catch (error) {
        console.error('Error downloading ticket PDF:', error);
        throw error;
    }
};