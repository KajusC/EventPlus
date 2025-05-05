import axios from 'axios';
import { isTokenExpired } from '../utils/authUtils';

// Create an axios instance
const apiClient = axios.create({
  baseURL: 'https://localhost:7244/api',
  headers: {
    'Content-Type': 'application/json',
  }
});

// Add a request interceptor to attach the token to every request
apiClient.interceptors.request.use(
  async config => {
    const token = localStorage.getItem('token');
    
    // If token exists and is not expired, add it to the request
    if (token && !isTokenExpired(token)) {
      config.headers.Authorization = `Bearer ${token}`;
    } else if (token && isTokenExpired(token)) {
      // Token is expired, clear it from storage
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      
      // If we had a refresh token mechanism, we would use it here
      // For now, we'll just clear the expired token
    }
    
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Add a response interceptor to handle common errors
apiClient.interceptors.response.use(
  response => response,
  error => {
    const originalRequest = error.config;
    
    // Handle 401 Unauthorized
    if (error.response && error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      // Clear storage on auth failure
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      
      // Redirect to login page if available
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    
    // Handle 403 Forbidden (insufficient permissions)
    if (error.response && error.response.status === 403) {
      console.error('Insufficient permissions for this operation');
      // Could redirect to a permission denied page
      // window.location.href = '/permission-denied';
    }
    
    // If the error has a response, let's extract the message for easier error handling
    if (error.response && error.response.data) {
      error.message = error.response.data.message || error.message;
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;