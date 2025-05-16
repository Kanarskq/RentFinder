import apiClient from './apiClient';

export const propertyApi = {

    getSimilarProperties: (similarParams) => {
        return apiClient.post('/api/property/similar', similarParams);
    },

    getPropertyById: (id) => {
        return apiClient.get(`/api/property/${id}`);
    },

    createProperty: (propertyData) => {
        return apiClient.post('/api/property', propertyData);
    },

    updateProperty: (id, propertyData) => {
        return apiClient.put(`/api/property/${id}`, propertyData);
    },

    addPropertyImage: (propertyId, imageFormData) => {
        return apiClient.post(`/api/property/${propertyId}/images`, imageFormData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    },

    deletePropertyImage: (propertyId, imageId) => {
        return apiClient.delete(`/api/property/${propertyId}/images/${imageId}`);
    },

    changePropertyStatus: (propertyId, isAvailable) => {
        if (isAvailable) {
            return apiClient.put(`/api/property/${propertyId}/status/available`);
        } else {
            return apiClient.put(`/api/property/${propertyId}/status/unavailable`);
        }
    },

    getAllProperties: () => {
        return apiClient.get('/api/property');
    },

    getPropertiesByStatus: (status) => {
        return apiClient.get(`/api/property/status/${status}`);
    },

    getPropertyStatistics: () => {
        return apiClient.get('/api/property/stats');
    }
};