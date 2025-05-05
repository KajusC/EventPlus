import React, { createContext, useContext, useState, useCallback } from 'react';

// Create context for notifications
const NotificationContext = createContext();

// Types of notifications
export const NOTIFICATION_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error',
  INFO: 'info',
  WARNING: 'warning',
};

// Provider component for notifications
export const NotificationProvider = ({ children }) => {
  const [notification, setNotification] = useState(null);

  // Show a notification
  const showNotification = useCallback((message, type = NOTIFICATION_TYPES.INFO, timeout = 5000) => {
    setNotification({ message, type });
    
    if (timeout) {
      setTimeout(() => {
        setNotification(null);
      }, timeout);
    }
  }, []);

  // Hide the notification
  const hideNotification = useCallback(() => {
    setNotification(null);
  }, []);

  // Helper methods for different notification types
  const showSuccess = useCallback((message, timeout) => {
    showNotification(message, NOTIFICATION_TYPES.SUCCESS, timeout);
  }, [showNotification]);

  const showError = useCallback((message, timeout) => {
    showNotification(message, NOTIFICATION_TYPES.ERROR, timeout);
  }, [showNotification]);

  const showInfo = useCallback((message, timeout) => {
    showNotification(message, NOTIFICATION_TYPES.INFO, timeout);
  }, [showNotification]);

  const showWarning = useCallback((message, timeout) => {
    showNotification(message, NOTIFICATION_TYPES.WARNING, timeout);
  }, [showNotification]);

  // Value to provide through the context
  const value = {
    notification,
    showNotification,
    hideNotification,
    showSuccess,
    showError,
    showInfo,
    showWarning,
  };

  return (
    <NotificationContext.Provider value={value}>
      {children}
      {notification && <NotificationDisplay notification={notification} onClose={hideNotification} />}
    </NotificationContext.Provider>
  );
};

// Hook to use notifications
export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error('useNotification must be used within a NotificationProvider');
  }
  return context;
};

// Component to display notifications
const NotificationDisplay = ({ notification, onClose }) => {
  const { message, type } = notification;

  // Define styles based on type
  const getBackgroundColor = () => {
    switch (type) {
      case NOTIFICATION_TYPES.SUCCESS:
        return '#4caf50';
      case NOTIFICATION_TYPES.ERROR:
        return '#f44336';
      case NOTIFICATION_TYPES.WARNING:
        return '#ff9800';
      case NOTIFICATION_TYPES.INFO:
      default:
        return '#2196f3';
    }
  };

  return (
    <div
      style={{
        position: 'fixed',
        top: '20px',
        right: '20px',
        zIndex: 9999,
        padding: '16px 24px',
        borderRadius: '4px',
        backgroundColor: getBackgroundColor(),
        color: 'white',
        boxShadow: '0 4px 6px rgba(0, 0, 0, 0.1)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        minWidth: '300px',
        maxWidth: '500px',
      }}
    >
      <div style={{ marginRight: '12px' }}>{message}</div>
      <button
        onClick={onClose}
        style={{
          background: 'transparent',
          border: 'none',
          color: 'white',
          cursor: 'pointer',
          fontWeight: 'bold',
          fontSize: '16px',
        }}
      >
        ×
      </button>
    </div>
  );
};

export default NotificationProvider;