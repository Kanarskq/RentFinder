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
    },

    createPaymentIntent: (data) => {
        return apiClient.post('/api/payment/create-payment-intent', data);
    },

    confirmPayment: (data) => {
        return apiClient.post('/api/payment/confirm-payment', data);
    }
};