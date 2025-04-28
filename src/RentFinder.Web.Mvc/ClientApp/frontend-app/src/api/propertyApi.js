import apiClient from './apiClient';

export const propertyApi = {
    getRecommendedProperties: () => {
        return apiClient.get('/properties/recommended');
    },

    searchProperties: (searchParams) => {
        return apiClient.get('/properties/search', { params: searchParams });
    },

    getPropertyById: (id) => {
        return apiClient.get(`/properties/${id}`);
    }
};