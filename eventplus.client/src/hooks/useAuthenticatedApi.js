import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';

/**
 * Custom hook for making authenticated API requests
 * @param {Function} apiFunction - The API function to call
 * @param {Array} dependencies - Dependencies that should trigger a refetch (like in useEffect)
 * @param {any} initialData - Initial data to use before the API call completes
 * @returns {Object} Object containing data, loading state, error, and refetch function
 */
export const useAuthenticatedApi = (apiFunction, dependencies = [], initialData = null) => {
  const [data, setData] = useState(initialData);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { isAuthenticated, refreshToken } = useAuth();

  const fetchData = async (...args) => {
    setLoading(true);
    setError(null);

    try {
      if (apiFunction.requiresAuth && !isAuthenticated()) {
        throw new Error('Authentication required');
      }

      if (apiFunction.requiresAuth) {
        await refreshToken();
      }

      const result = await apiFunction(...args);
      setData(result);
      return result;
    } catch (err) {
      setError(err);
      console.error('API Error:', err);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (dependencies.length > 0) {
      fetchData(...dependencies).catch(err => {
      });
    } else {
      fetchData().catch(err => {
      });
    }
  }, [isAuthenticated, ...dependencies]);

  const refetch = async (...args) => {
    return fetchData(...args);
  };

  return { data, loading, error, refetch };
};

export default useAuthenticatedApi;