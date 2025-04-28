import React from 'react';
import { useParams } from 'react-router-dom';
import { useBookings } from '../../hooks/useBookings';
import { formatDate, formatCurrency } from '../../utils/formatters';

const BookingDetail = () => {
    const { id } = useParams();
    const { booking, loading, error, cancelBooking } = useBookings(id);

    if (loading) return <div>Loading booking details...</div>;
    if (error) return <div>Error: {error}</div>;
    if (!booking) return <div>Booking not found</div>;

    const handleCancel = async () => {
        if (window.confirm('Are you sure you want to cancel this booking?')) {
            await cancelBooking(id);
        }
    };

    return (
        <div className="booking-detail">
            <h2>Booking #{booking.bookingReference}</h2>

            <div className="booking-status">
                <span className={`status-badge ${booking.status.toLowerCase()}`}>
                    {booking.status}
                </span>
                <p>Booked on: {formatDate(booking.bookingDate)}</p>
            </div>

            <div className="booking-property">
                <h3>Property Details</h3>
                <div className="property-card">
                    <img src={booking.property.imageUrl} alt={booking.property.title} />
                    <div className="property-info">
                        <h4>{booking.property.title}</h4>
                        <p>{booking.property.address}</p>
                        <p>{booking.property.bedrooms} beds • {booking.property.bathrooms} baths</p>
                    </div>
                </div>
            </div>

            <div className="booking-dates">
                <h3>Dates</h3>
                <div className="date-range">
                    <div className="date-item">
                        <span className="date-label">Check-in:</span>
                        <span className="date-value">{formatDate(booking.checkInDate)}</span>
                    </div>
                    <div className="date-item">
                        <span className="date-label">Check-out:</span>
                        <span className="date-value">{formatDate(booking.checkOutDate)}</span>
                    </div>
                    <div className="date-item">
                        <span className="date-label">Total nights:</span>
                        <span className="date-value">
                            {Math.ceil((new Date(booking.checkOutDate) - new Date(booking.checkInDate)) / (1000 * 60 * 60 * 24))}
                        </span>
                    </div>
                </div>
            </div>

            <div className="booking-payment">
                <h3>Payment Summary</h3>
                <div className="payment-details">
                    <div className="payment-item">
                        <span className="payment-label">Total:</span>
                        <span className="payment-value">{formatCurrency(booking.totalPrice)}</span>
                    </div>
                    <div className="payment-item">
                        <span className="payment-label">Payment Method:</span>
                        <span className="payment-value">Credit Card (ending in 1234)</span>
                    </div>
                    <div className="payment-item">
                        <span className="payment-label">Payment Status:</span>
                        <span className="payment-value">Paid</span>
                    </div>
                </div>
            </div>

            {booking.canCancel && (
                <div className="booking-actions">
                    <button
                        onClick={handleCancel}
                        className="cancel-button"
                        disabled={booking.status === 'CANCELLED'}
                    >
                        {booking.status === 'CANCELLED' ? 'Cancelled' : 'Cancel Booking'}
                    </button>
                </div>
            )}
        </div>
    );
};

export default BookingDetail;