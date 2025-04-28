import React from 'react';
import BookingList from '../components/bookings/BookingList';
import { useAuth } from '../hooks/useAuth';

const BookingsPage = () => {
    const { isAuthenticated } = useAuth();

    if (!isAuthenticated) {
        return (
            <div className="auth-required">
                <h2>Please log in to view your bookings</h2>
            </div>
        );
    }

    return (
        <div className="bookings-page">
            <h1>Your Bookings</h1>
            <BookingList />
        </div>
    );
};

export default BookingsPage;