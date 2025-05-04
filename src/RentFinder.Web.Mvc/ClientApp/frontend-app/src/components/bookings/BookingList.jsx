import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useBookings } from '../../hooks/useBookings';
import { useAuth } from '../../hooks/useAuth';

const BookingList = () => {
    const { bookings, loading, error, refreshBookings } = useBookings();
    const { currentUser } = useAuth();

    useEffect(() => {
        console.log("Current user in BookingList:", currentUser);
        console.log("Bookings data:", bookings);
        console.log("Loading state:", loading);
        console.log("Error state:", error);
    }, [currentUser, bookings, loading, error]);

    const handleManualRefresh = () => {
        console.log("Manually refreshing bookings...");
        refreshBookings();
    };

    if (loading) return <div className="loading">Loading your bookings...</div>;
    
    if (error) return (
        <div className="error-message">
            <p>Error: {error}</p>
            <button onClick={handleManualRefresh} className="refresh-btn">Retry</button>
        </div>
    );
    
    if (!bookings || bookings.length === 0) {
        return (
            <div className="no-bookings">
                <p>You don't have any bookings yet.</p>
                <p>User email: {currentUser?.email || "No email available"}</p>
                <button onClick={handleManualRefresh} className="refresh-btn">Refresh Bookings</button>
                <Link to="/" className="browse-properties-btn">Browse Properties</Link>
            </div>
        );
    }

    const getStatusClass = (status) => {
        switch (status) {
            case 'Confirmed': return 'status-confirmed';
            case 'Paid': return 'status-paid';
            case 'Cancelled': return 'status-cancelled';
            case 'Completed': return 'status-completed';
            default: return 'status-pending';
        }
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString();
    };

    return (
        <div className="booking-list">
            <h2>Your Bookings</h2>
            <button onClick={handleManualRefresh} className="refresh-btn">Refresh</button>
            {bookings.map(booking => (
                <div key={booking.id} className="booking-card">
                    <div className="booking-info">
                        <h3>Booking #{booking.id}</h3>
                        <div className="booking-details">
                            <p>
                                <strong>Dates:</strong> {formatDate(booking.startDate)} - {formatDate(booking.endDate)}
                            </p>
                            <p>
                                <strong>Total:</strong> ${booking.totalPrice.toFixed(2)}
                            </p>
                            <p>
                                <strong>Status:</strong>
                                <span className={`status-badge ${getStatusClass(booking.status)}`}>
                                    {booking.status}
                                </span>
                            </p>
                        </div>
                        <Link to={`/bookings/${booking.id}`} className="view-details-btn">
                            View Details
                        </Link>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default BookingList;