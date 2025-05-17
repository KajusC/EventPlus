import apiClient from './apiClient';

const API_ENDPOINT = '/userRequestAnswer';

export const fetchUserRequestAnswers = async () => {
    try {
        const response = await apiClient.get(API_ENDPOINT);
        return response.data;
    } catch (error) {
        console.error('Error fetching user requests:', error);
        throw error;
    }
};

export const fetchUserRequestAnswerById = async (id) => {
    try {
        const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Error fetching user request with id ${id}:`, error);
        throw error;
    }
};

export const fetchUserRequestAnswersByUserId = async (userId) => {
    try {
        const response = await apiClient.get(`${API_ENDPOINT}/user/${userId}`);
        return response.data;
    } catch (error) {
        console.error(`Error fetching user requests for user ${userId}:`, error);
        throw error;
    }
};

export const createUserRequestAnswer = async (userRequestAnswerData) => {
    try {
        const formattedData = {
            IdUserRequestAnswer: userRequestAnswerData.IdUserRequestAnswer || 0,
            Answer: userRequestAnswerData.Answer,
            FkQuestionidQuestion: userRequestAnswerData.FkQuestionidQuestion,
            FkUseridUser: userRequestAnswerData.FkUseridUser
        };
        
        console.log('Siunčiami duomenys į serverį:', formattedData);
        const response = await apiClient.post(API_ENDPOINT, formattedData);
        return response.data;
    } catch (error) {
        console.error('Klaida kuriant klausimą:', error);
        if (error.response) {
            console.error('Serverio atsakymas:', error.response.data);
            if (error.response.data && error.response.data.errors) {
                console.error('Validavimo klaidos:', error.response.data.errors);
            }
        }
        throw error;
    }
};

export const updateUserRequestAnswer = async (UserRequestAnswerData) => {
    try {
        const response = await apiClient.put(API_ENDPOINT, UserRequestAnswerData);
        return response.data;
    } catch (error) {
        console.error('Error updating user request:', error);
        throw error;
    }
};

export const deleteUserRequestAnswer = async (id) => {
    try {
        const response = await apiClient.delete(`${API_ENDPOINT}/${id}`);
        return response.data;
    } catch (error) {
        console.error(`Error deleting user request with id ${id}:`, error);
        throw error;
    }
};