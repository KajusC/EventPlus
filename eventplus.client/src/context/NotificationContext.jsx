import React, { createContext, useContext, useState, useCallback } from 'react';

const NotificationContext = createContext();

export const NOTIFICATION_TYPES = {
  SUCCESS: 'success',
  ERROR: 'error',
  INFO: 'info',
  WARNING: 'warning',
};

export const NotificationProvider = ({ children }) => {
  const [notification, setNotification] = useState(null);

  const showNotification = useCallback((message, type = NOTIFICATION_TYPES.INFO, timeout = 5000) => {
    setNotification({ message, type });
    
    if (timeout) {
      setTimeout(() => {
        setNotification(null);
      }, timeout);
    }
  }, []);

  const hideNotification = useCallback(() => {
    setNotification(null);
  }, []);

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

export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error('useNotification must be used within a NotificationProvider');
  }
  return context;
};

const NotificationDisplay = ({ notification, onClose }) => {
  const { message, type } = notification;

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