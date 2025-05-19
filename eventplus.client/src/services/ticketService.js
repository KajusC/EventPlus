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

export const scanTicket = async (qrCode) => {
  const response = await Promise.race([
    apiClient.post(`${API_ENDPOINT}/ScanQrCode`, qrCode),
    new Promise((_, reject) => 
      setTimeout(() => reject(new Error('Scan timeout after 5 seconds')), 5000)
    )
  ]);
  return response.data;
};

export const adjustAllEventPrices = async () => {
  try {
    const response = await apiClient.post(`${API_ENDPOINT}/StartPriceUpdate`);
    return response.data;
  } catch (error) {
    console.error('Error adjusting all prices:', error);
    throw new Error(error.response?.data?.message || 'Failed to adjust all prices');
  }
}