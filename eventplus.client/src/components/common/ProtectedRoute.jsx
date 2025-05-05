import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const ProtectedRoute = ({ requireAdmin = false, requireOrganizer = false }) => {
  const { isAuthenticated, isAdmin, isOrganizer } = useAuth();
  
  // If not authenticated, redirect to login
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }
  
  // If requireAdmin is true and user is not admin, redirect to home
  if (requireAdmin && !isAdmin()) {
    return <Navigate to="/" replace />;
  }
  
  // If requireOrganizer is true and user is not organizer, redirect to home
  if (requireOrganizer && !isOrganizer()) {
    return <Navigate to="/" replace />;
  }
  
  // If authenticated (and admin/organizer if required), render the children
  return <Outlet />;
};

export default ProtectedRoute;