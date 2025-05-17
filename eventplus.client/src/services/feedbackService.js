import apiClient from './apiClient';

const API_ENDPOINT = '/Feedback';

export const fetchFeedbacksByEventId = async (eventId) => {
    try {
        const response = await apiClient.get(`${API_ENDPOINT}/event/${eventId}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching feedbacks:', error);
        throw error;
    }
};

export const createFeedback = async (feedbackData) => {
    try {
        const response = await apiClient.post(API_ENDPOINT, feedbackData);
        return response.data;
    } catch (error) {
        console.error('Error creating feedback:', error);
        throw error;
    }
};

export const updateFeedback = async (feedback) => {
    try {
        const response = await apiClient.put(API_ENDPOINT, feedback);
        return response.data;
    } catch (error) {
        console.error('Error updating feedback:', error);
        throw error;
    }
};

export const deleteFeedback = async (id) => {
    try {
        const response = await apiClient.delete(`${API_ENDPOINT}/${id}`);
        return response.data;
    } catch (error) {
        console.error('Error deleting feedback:', error);
        throw error;
    }
};