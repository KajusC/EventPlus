import apiClient from './apiClient';

const API_ENDPOINT = '/Category';

export const fetchCategories = async () => {
  try {
    const response = await apiClient.get(API_ENDPOINT);
    return response.data;
  } catch (error) {
    console.error("Error fetching categories:", error);
    throw error;
  }
};

export const fetchCategoryById = async (id) => {
  try {
    if (!id || isNaN(parseInt(id))) {
      return { name: "Unknown Category" };
    }
    const response = await apiClient.get(`${API_ENDPOINT}/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching category with ID ${id}:`, error);
    return { name: "Unknown Category" };
  }
};

export const getCategoryNameById = (categories, categoryId) => {
  if (!categories || !categoryId) return "Unknown Category";
  
  const category = categories.find(cat => cat.idCategory === categoryId);
  return category ? category.name : "Unknown Category";
};