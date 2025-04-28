import React, { useState } from 'react';
import { useAuth } from '../../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import { bookingApi } from '../../api/bookingApi';

const BookingForm = ({ propertyId, price }) => {
    const { isAuthenticated } = useAuth();
    const navigate = useNavigate();
    const [bookingData, setBookingData] = useState({
        propertyId,
        startDate: '',
        endDate: '',
        guestCount: 1,
        specialRequests: ''
    });
    const [error, setError] = useState('');
    const [success, setSuccess] = useState(false);
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setBookingData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const calculateTotal = () => {
        if (!bookingData.startDate || !bookingData.endDate) return 0;

        const start = new Date(bookingData.startDate);
        const end = new Date(bookingData.endDate);
        const days = (end - start) / (1000 * 60 * 60 * 24);

        return days * price;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const response = await bookingApi.createBooking(bookingData);
            if (response.data.success) {
                setSuccess(true);
                setTimeout(() => {
                    navigate(`/bookings/${response.data.bookingId}`);
                }, 2000);
            }
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to create booking');
        } finally {
            setLoading(false);
        }
    };

    if (!isAuthenticated) {
        return (
            <div className="booking-login-prompt">
                <p>Please log in to make a booking</p>
            </div>
        );
    }

    if (success) {
        return (
            <div className="booking-success">
                <p>Booking created successfully! Redirecting...</p>
            </div>
        );
    }

    return (
        <form className="booking-form" onSubmit={handleSubmit}>
            <h3>Book This Property</h3>

            <div className="form-group">
                <label htmlFor="startDate">Check-in Date</label>
                <input
                    type="date"
                    id="startDate"
                    name="startDate"
                    value={bookingData.startDate}
                    onChange={handleChange}
                    required
                    min={new Date().toISOString().split('T')[0]}
                />
            </div>

            <div className="form-group">
                <label htmlFor="endDate">Check-out Date</label>
                <input
                    type="date"
                    id="endDate"
                    name="endDate"
                    value={bookingData.endDate}
                    onChange={handleChange}
                    required
                    min={bookingData.startDate || new Date().toISOString().split('T')[0]}
                />
            </div>

            <div className="form-group">
                <label htmlFor="guestCount">Number of Guests</label>
                <input
                    type="number"
                    id="guestCount"
                    name="guestCount"
                    value={bookingData.guestCount}
                    onChange={handleChange}
                    min="1"
                    max="10"
                    required
                />
            </div>

            <div className="form-group">
                <label htmlFor="specialRequests">Special Requests</label>
                <textarea
                    id="specialRequests"
                    name="specialRequests"
                    value={bookingData.specialRequests}
                    onChange={handleChange}
                    rows="3"
                />
            </div>

            <div className="booking-summary">
                <h4>Booking Summary</h4>
                <p>Price per night: ${price}</p>
                <p>Total: ${calculateTotal()}</p>
            </div>

            {error && <div className="error-message">{error}</div>}

            <button type="submit" disabled={loading} className="book-button">
                {loading ? 'Processing...' : 'Book Now'}
            </button>
        </form>
    );
};

export default BookingForm;