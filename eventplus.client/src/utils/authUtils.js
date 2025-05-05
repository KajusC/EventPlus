/**
 * Utilities for handling authentication tokens
 */

/**
 * Parse a JWT token to extract information
 * @param {string} token - The JWT token to parse
 * @returns {object} The token data or null if invalid
 */
export const parseJwt = (token) => {
  if (!token) return null;
  
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map(c => `%${('00' + c.charCodeAt(0).toString(16)).slice(-2)}`)
        .join('')
    );

    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error parsing JWT token:', error);
    return null;
  }
};

/**
 * Check if a JWT token is expired
 * @param {string} token - The JWT token to check
 * @returns {boolean} True if token is expired or invalid, false otherwise
 */
export const isTokenExpired = (token) => {
  if (!token) return true;
  
  try {
    const decoded = parseJwt(token);
    if (!decoded) return true;
    
    const expirationTime = decoded.exp * 1000;
    const currentTime = Date.now();
    
    return currentTime >= expirationTime;
  } catch (error) {
    console.error('Error checking token expiration:', error);
    return true;
  }
};

/**
 * Get user role from token
 * @param {string} token - The JWT token
 * @returns {string|null} User role or null if token is invalid
 */
export const getUserRoleFromToken = (token) => {
  if (!token) return null;
  
  try {
    const decoded = parseJwt(token);
    return decoded?.role || null;
  } catch (error) {
    console.error('Error getting user role from token:', error);
    return null;
  }
};