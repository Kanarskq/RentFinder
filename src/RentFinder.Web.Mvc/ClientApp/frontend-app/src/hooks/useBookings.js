import { useState, useEffect } from 'react';
import { bookingApi } from '../api/bookingApi';

export const useBookings = (id) => {
    const [bookings, setBookings] = useState([]);
    const [booking, setBooking] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const fetchUserBookings = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await bookingApi.getUserBookings();
            setBookings(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch bookings');
        } finally {
            setLoading(false);
        }
    };

    const fetchBookingById = async (id) => {
        setLoading(true);
        setError(null);
        try {
            const response = await bookingApi.getBookingById(id);
            setBooking(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch booking');
        } finally {
            setLoading(false);
        }
    };

    const cancelBooking = async (id) => {
        setLoading(true);
        setError(null);
        try {
            await bookingApi.cancelBooking(id);
            await fetchBookingById(id);
            return true;
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to cancel booking');
            return false;
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) {
            fetchBookingById(id);
        } else {
            fetchUserBookings();
        }
    }, [id]);

    return {
        bookings,
        booking,
        loading,
        error,
        cancelBooking,
        refreshBookings: fetchUserBookings
    };
};