import apiClient from './apiClient';

export const bookingApi = {
    getUserBookings: (id) => {
        return apiClient.get(`/api/booking/user/${id}`);
    },

    getBookingById: (id) => {
        return apiClient.get(`/api/booking/${id}`);
    },

    createBooking: (bookingData) => {
        return apiClient.post('/api/booking', bookingData);
    },

    cancelBooking: (id) => {
        return apiClient.delete(`/api/booking/${id}`);
    }
};