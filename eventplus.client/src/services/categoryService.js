import apiClient from './apiClient';

const API_ENDPOINT = '/Category';

// Fetch all categories
export const fetchCategories = async () => {
  try {
    const response = await apiClient.get(API_ENDPOINT);
    return response.data;
  } catch (error) {
    console.error("Error fetching categories:", error);
    throw error;
  }
};

// Fetch category by ID (numeric ID)
export const fetchCategoryById = async (id) => {
  try {
    if (!id || isNaN(parseInt(id))) {
      return { name: "Unknown Category" }; // Fallback for invalid IDs
    }
    const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching category with ID ${id}:`, error);
    return { name: "Unknown Category" }; // Fallback on error
  }
};

// Find category name from local categories array
export const getCategoryNameById = (categories, categoryId) => {
  if (!categories || !categoryId) return "Unknown Category";
  
  const category = categories.find(cat => cat.idCategory === categoryId);
  return category ? category.name : "Unknown Category";
};