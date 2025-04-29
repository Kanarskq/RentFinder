import apiClient from './apiClient';

export const reviewApi = {
    getReviewById: (reviewId) => {
        return apiClient.get(`/api/review/${reviewId}`);
    },

    getReviewsByProperty: (propertyId) => {
        return apiClient.get(`/api/review/property/${propertyId}`);
    },

    getReviewsByUser: (userId) => {
        return apiClient.get(`/api/review/user/${userId}`);
    },

    getAveragePropertyRating: (propertyId) => {
        return apiClient.get(`/api/review/property/${propertyId}/rating`);
    },

    createReview: (reviewData) => {
        return apiClient.post('/api/review', reviewData);
    },

    updateReview: (reviewId, reviewData) => {
        return apiClient.put(`/api/review/${reviewId}`, reviewData);
    }
};