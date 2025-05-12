import axios from 'axios';
import { getToken } from './authService';

const BASE_URL = 'https://localhost:7244';

const getAuthHeaders = () => {
    const token = getToken(); // implement this to get your JWT
    return token ? { Authorization: `Bearer ${token}` } : {};
};

export const fetchFeedbacksByEventId = async (eventId) => {
    try {
        const response = await axios.get(`${BASE_URL}/api/Feedback/event/${eventId}`);
        console.log("API Response:", response); // Debugging
        return response.data;
    } catch (error) {
        console.error('Error fetching feedbacks:', error);
        throw error;
    }
};

export const createFeedback = async (feedbackData) => {
    try {
        const response = await axios.post(`${BASE_URL}/api/Feedback`, feedbackData);
        return response.data;
    } catch (error) {
        console.error('Error creating feedback:', error);
        throw error;
    }
}

export const updateFeedback = async (feedback) => {
    try {
        const response = await axios.put(
            `${BASE_URL}/api/Feedback`,
            feedback,
            { headers: { ...getAuthHeaders() } }
        );
        return response.data;
    } catch (error) {
        console.error('Error updating feedback:', error);
        throw error;
    }
};

export const deleteFeedback = async (id) => {
    try {
        const response = await axios.delete(
            `${BASE_URL}/api/Feedback/${id}`,
            { headers: { ...getAuthHeaders() } }
        );
        return response.data;
    } catch (error) {
        console.error('Error deleting feedback:', error);
        throw error;
    }
};