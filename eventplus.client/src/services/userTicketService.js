import apiClient from './apiClient';

const API_ENDPOINT = '/UserTicket';

export const createUserTicket = async (userId, ticketId) => {
  const response = await apiClient.post(API_ENDPOINT, {
    fkUseridUser: userId,
    fkTicketidTicket: ticketId
  });
  return response.data;
};

export const fetchTicketsByUserId = async (userId) => {
  const response = await apiClient.get(`${API_ENDPOINT}/${userId}`);
  return response.data;
};