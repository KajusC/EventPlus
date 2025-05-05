import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useNotification } from '../context/NotificationContext';

/**
 * Higher-order component (HOC) to protect components with authentication
 * @param {React.Component} Component - The component to wrap with authentication
 * @param {Object} options - Configuration options
 * @param {boolean} options.requireAuth - Whether authentication is required
 * @param {boolean} options.requireAdmin - Whether admin privileges are required
 * @param {boolean} options.requireOrganizer - Whether organizer privileges are required
 * @returns {React.Component} Protected component
 */
const withAuth = (Component, { requireAuth = true, requireAdmin = false, requireOrganizer = false } = {}) => {
  const ProtectedComponent = (props) => {
    const { isAuthenticated, isAdmin, isOrganizer } = useAuth();
    const { showError } = useNotification();
    const navigate = useNavigate();

    useEffect(() => {
      if (requireAuth && !isAuthenticated()) {
        showError('Please log in to access this feature');
        navigate('/login', { state: { from: window.location } });
      } else if (requireAdmin && !isAdmin()) {
        showError('You do not have permission to access this feature');
        navigate('/');
      } else if (requireOrganizer && !isOrganizer()) {
        showError('Only event organizers can access this feature');
        navigate('/');
      }
    }, [isAuthenticated, isAdmin, isOrganizer, navigate, showError]);

    if (requireAuth && !isAuthenticated()) {
      return null; // Don't render anything while redirecting
    }

    if (requireAdmin && !isAdmin()) {
      return null; // Don't render anything while redirecting
    }

    if (requireOrganizer && !isOrganizer()) {
      return null; // Don't render anything while redirecting
    }

    return <Component {...props} />;
  };

  return ProtectedComponent;
};

export default withAuth;