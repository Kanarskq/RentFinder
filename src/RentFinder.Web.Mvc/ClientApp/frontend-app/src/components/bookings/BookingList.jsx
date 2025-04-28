import React from 'react';
import { Link } from 'react-router-dom';
import { useBookings } from '../../hooks/useBookings';

const BookingList = () => {
    const { bookings, loading, error } = useBookings();

    if (loading) return <div>Loading your bookings...</div>;
    if (error) return <div>Error: {error}</div>;
    if (!bookings || bookings.length === 0) {
        return <div>You don't have any bookings yet.</div>;
    }

    return (
        <div className="booking-list">
            <h2>Your Bookings</h2>
            {bookings.map(booking => (
                <div key={booking.id} className="booking-card">
                    <div className="booking-image">
                        <img src={booking.propertyImageUrl} alt={booking.propertyTitle} />
                    </div>
                    <div className="booking-info">
                        <h3>{booking.propertyTitle}</h3>
                        <p>Dates: {new Date(booking.startDate).toLocaleDateString()} - {new Date(booking.endDate).toLocaleDateString()}</p>
                        <p>Total: ${booking.totalPrice}</p>
                        <p>Status: {booking.status}</p>
                        <Link to={`/bookings/${booking.id}`} className="view-details">
                            View Details
                        </Link>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default BookingList;