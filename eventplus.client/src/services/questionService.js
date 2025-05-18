import apiClient from './apiClient';

const API_ENDPOINT = '/Question';

export const fetchQuestions = async () => {
    try {
        const response = await apiClient.get(API_ENDPOINT);
        return response.data;
    } catch (error) {
        console.error('Klaida gaunant klausimus:', error);
        throw error;
    }
};

export const fetchQuestionById = async (id) => {
    try {
        const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Klaida gaunant klausimą #${id}:`, error);
        throw error;
    }
};

export const createQuestion = async (questionData) => {
    try {
        const response = await apiClient.post(API_ENDPOINT, questionData);
        return response.data;
    } catch (error) {
        console.error('Klaida kuriant klausimą:', error);
        throw error;
    }
};

export const updateQuestion = async (questionData) => {
    try {
        const response = await apiClient.put(API_ENDPOINT, questionData);
        return response.data;
    } catch (error) {
        console.error('Klaida atnaujinant klausimą:', error);
        throw error;
    }
};

export const deleteQuestion = async (id) => {
    try {
        const response = await apiClient.delete(`${API_ENDPOINT}/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Klaida trinant klausimą #${id}:`, error);
        throw error;
    }
};