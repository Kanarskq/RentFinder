import { useState, useEffect } from 'react';
import { bookingApi } from '../api/bookingApi';
import { useAuth } from './useAuth';

export const useBookings = (id) => {
    const [bookings, setBookings] = useState([]);
    const [booking, setBooking] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const { currentUser } = useAuth();

    const fetchUserBookings = async () => {
        if (!currentUser?.id) {
            setError('User not authenticated');
            return;
        }

        setLoading(true);
        setError(null);
        try {
            console.error('currentUser.id', currentUser.id);
            const response = await bookingApi.getUserBookings(currentUser.id);
            setBookings(response.data);
        } catch (err) {
            console.error('Error fetching bookings:', err);
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
            console.error('Error fetching booking:', err);
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
            if (booking) {
                setBooking({
                    ...booking,
                    status: 'Cancelled'
                });
            }
            return true;
        } catch (err) {
            console.error('Error cancelling booking:', err);
            setError(err.response?.data?.message || 'Failed to cancel booking');
            return false;
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) {
            fetchBookingById(id);
        } else if (currentUser?.id) {
            fetchUserBookings();
        }
    }, [id, currentUser?.id]);

    return {
        bookings,
        booking,
        loading,
        error,
        cancelBooking,
        refreshBookings: fetchUserBookings
    };
};