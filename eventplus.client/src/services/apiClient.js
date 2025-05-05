import axios from 'axios';
import { isTokenExpired } from '../utils/authUtils';

const apiClient = axios.create({
  baseURL: 'https://localhost:7244/api',
  headers: {
    'Content-Type': 'application/json',
  }
});

apiClient.interceptors.request.use(
  async config => {
    const token = localStorage.getItem('token');
    
    if (token && !isTokenExpired(token)) {
      config.headers.Authorization = `Bearer ${token}`;
    } else if (token && isTokenExpired(token)) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
    }
    
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  response => response,
  error => {
    const originalRequest = error.config;
    
    if (error.response && error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    
    if (error.response && error.response.status === 403) {
      console.error('Insufficient permissions for this operation');
    }
    
    if (error.response && error.response.data) {
      error.message = error.response.data.message || error.message;
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;