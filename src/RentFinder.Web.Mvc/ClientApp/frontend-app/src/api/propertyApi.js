import apiClient from './apiClient';

export const propertyApi = {

    getSimilarProperties: (similarParams) => {
        return apiClient.post('/api/property/similar', similarParams);
    },

    getPropertyById: (id) => {
        return apiClient.get(`/api/property/${id}`);
    }
};