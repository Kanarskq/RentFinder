import apiClient from './apiClient';

export const bookingApi = {
    getUserBookings: () => {
        return apiClient.get('/bookings');
    },

    getBookingById: (id) => {
        return apiClient.get(`/bookings/${id}`);
    },

    createBooking: (bookingData) => {
        return apiClient.post('/bookings', bookingData);
    },

    cancelBooking: (id) => {
        return apiClient.post(`/bookings/${id}/cancel`);
    }
};
