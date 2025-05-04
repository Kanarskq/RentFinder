import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useBookings } from '../../hooks/useBookings';

const BookingDetail = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { booking, loading, error, cancelBooking } = useBookings(id);

    if (loading) return <div className="loading">Loading booking details...</div>;
    if (error) return <div className="error-message">Error: {error}</div>;
    if (!booking) return <div className="not-found">Booking not found</div>;

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString();
    };

    const handleCancel = async () => {
        if (window.confirm('Are you sure you want to cancel this booking?')) {
            const success = await cancelBooking(id);
            if (success) {
                alert('Booking cancelled successfully');
                navigate('/bookings');
            }
        }
    };

    const canBeCancelled = booking.status === 'Pending' || booking.status === 'Confirmed';

    return (
        <div className="booking-detail">
            <h2>Booking Details</h2>

            <div className="booking-card detailed">
                <div className="booking-header">
                    <h3>Booking #{booking.id}</h3>
                    <span className={`status-badge status-${booking.status.toLowerCase()}`}>
                        {booking.status}
                    </span>
                </div>

                <div className="booking-info-section">
                    <h4>Dates</h4>
                    <div className="dates-group">
                        <div className="date-item">
                            <span>Check-in:</span>
                            <strong>{formatDate(booking.startDate)}</strong>
                        </div>
                        <div className="date-item">
                            <span>Check-out:</span>
                            <strong>{formatDate(booking.endDate)}</strong>
                        </div>
                    </div>
                </div>

                <div className="booking-info-section">
                    <h4>Property</h4>
                    <p>Property ID: {booking.propertyId}</p>
                </div>

                <div className="booking-info-section">
                    <h4>Payment Details</h4>
                    <p>Total Price: ${booking.totalPrice.toFixed(2)}</p>
                    {booking.paymentStatus && (
                        <p>Payment Status: {booking.paymentStatus}</p>
                    )}
                    {booking.paymentMethod && (
                        <p>Payment Method: {booking.paymentMethod}</p>
                    )}
                    {booking.paymentDate && (
                        <p>Payment Date: {formatDate(booking.paymentDate)}</p>
                    )}
                </div>

                <div className="booking-created">
                    <p>Booking created on {formatDate(booking.createdAt)}</p>
                </div>

                {canBeCancelled && (
                    <div className="booking-actions">
                        <button
                            onClick={handleCancel}
                            className="cancel-button"
                        >
                            Cancel Booking
                        </button>
                    </div>
                )}

                <div className="back-button-container">
                    <button
                        onClick={() => navigate('/bookings')}
                        className="back-button"
                    >
                        Back to All Bookings
                    </button>
                </div>
            </div>
        </div>
    );
};

export default BookingDetail;