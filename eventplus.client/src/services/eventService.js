import axios from 'axios';

const API_URL = 'https://localhost:7244/api/Event';

export const fetchEvents = async () => {
    const response = await axios.get(API_URL);
    return response.data;
};

export const fetchEventById = async (id) => {
    const response = await axios.get(`${API_URL}/${id}`);
    return response.data;
};

export const createEvent = async (eventData) => {
    const response = await axios.post(API_URL, eventData);
    return response.data;
};

export const updateEvent = async (id, eventData) => {
    console.log(eventData);
    const response = await axios.put(`${API_URL}/${id}`, eventData);
    console.log(response.data);
    return response.data;
};

export const deleteEvent = async (id) => {
    const response = await axios.delete(`${API_URL}/${id}`);
    console.log(response.data);
    return response.data;
};