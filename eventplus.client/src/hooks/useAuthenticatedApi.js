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
      // Check authentication status before making request
      if (apiFunction.requiresAuth && !isAuthenticated()) {
        throw new Error('Authentication required');
      }

      // Optional: refresh the token if needed
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
    // Only run the effect if we have arguments to pass to the API function
    if (dependencies.length > 0) {
      fetchData(...dependencies).catch(err => {
        // Already handled in fetchData
      });
    } else {
      // If no dependencies, just call the function with no args
      fetchData().catch(err => {
        // Already handled in fetchData
      });
    }
  }, [isAuthenticated, ...dependencies]);

  // Return a refetch function that can be called manually
  const refetch = async (...args) => {
    return fetchData(...args);
  };

  return { data, loading, error, refetch };
};

export default useAuthenticatedApi;