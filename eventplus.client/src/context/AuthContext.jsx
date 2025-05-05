import React, { createContext, useContext, useState, useEffect } from 'react';
import { isTokenExpired, parseJwt } from '../utils/authUtils';

const AuthContext = createContext();

export const useAuth = () => {
  return useContext(AuthContext);
};

export const AuthProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token') || null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Check if token is expired
    if (token && isTokenExpired(token)) {
      // If token is expired, clear auth state
      logout();
    } else if (token) {
      // Token is valid, load user data
      const userData = localStorage.getItem('user');
      if (userData) {
        setCurrentUser(JSON.parse(userData));
      }
    }
    setLoading(false);
  }, [token]);

  // Store token in localStorage when it changes
  useEffect(() => {
    if (token) {
      localStorage.setItem('token', token);
    } else {
      localStorage.removeItem('token');
    }
  }, [token]);

  const login = (userData) => {
    setCurrentUser(userData);
    setToken(userData.token);
    localStorage.setItem('user', JSON.stringify(userData));
  };

  const logout = () => {
    setCurrentUser(null);
    setToken(null);
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  };

  const isAuthenticated = () => {
    return !!token && !isTokenExpired(token);
  };

  const isAdmin = () => {
    if (!currentUser || !token || isTokenExpired(token)) return false;
    return currentUser.role === 'Admin';
  };

  const isOrganizer = () => {
    if (!currentUser || !token || isTokenExpired(token)) return false;
    return currentUser.role === 'Organiser';
  };

  const refreshToken = async () => {
    // Implement token refresh logic here if your API supports it
    // This would typically call an endpoint like /refreshToken
    // For now we'll just handle token expiration
    if (token && isTokenExpired(token)) {
      logout();
      return false;
    }
    return true;
  };

  const value = {
    currentUser,
    login,
    logout,
    isAuthenticated,
    isAdmin,
    isOrganizer,
    token,
    refreshToken
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};