import React, { useState } from 'react';
import { useAuth } from '../../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import { bookingApi } from '../../api/bookingApi';

const BookingForm = ({ propertyId, price }) => {
    const { isAuthenticated, currentUser } = useAuth();
    const navigate = useNavigate();
    const [bookingData, setBookingData] = useState({
        propertyId: parseInt(propertyId),
        userId: currentUser?.id,
        startDate: '',
        endDate: '',
        totalPrice: 0,
        createdAt: new Date().toISOString(),
        status: 'Pending'
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

        if (name === 'startDate' || name === 'endDate') {
            calculateTotal();
        }
    };

    const calculateTotal = () => {
        if (!bookingData.startDate || !bookingData.endDate) return 0;

        const start = new Date(bookingData.startDate);
        const end = new Date(bookingData.endDate);
        const days = Math.max(1, Math.ceil((end - start) / (1000 * 60 * 60 * 24)));
        const total = days * price;

        setBookingData(prev => ({
            ...prev,
            totalPrice: total
        }));

        return total;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        if (!bookingData.userId && currentUser?.id) {
            setBookingData(prev => ({
                ...prev,
                userId: currentUser.id
            }));
        }

        try {
            const formattedData = {
                propertyId: parseInt(propertyId),
                userId: parseInt(currentUser?.id || bookingData.userId),
                startDate: new Date(bookingData.startDate).toISOString(),
                endDate: new Date(bookingData.endDate).toISOString(),
                totalPrice: bookingData.totalPrice,
                createdAt: new Date().toISOString(),
                status: 'Pending'
            };

            const response = await bookingApi.createBooking(formattedData);
            setSuccess(true);
            setTimeout(() => {
                navigate('/bookings');
            }, 2000);
        } catch (err) {
            console.error('Booking error:', err);
            setError(err.response?.data?.message || 'Failed to create booking');
        } finally {
            setLoading(false);
        }
    };

    if (!isAuthenticated) {
        return (
            <div className="booking-login-prompt">
                <p>Please log in to make a booking</p>
                <button
                    onClick={() => navigate('/login')}
                    className="login-button"
                >
                    Log In
                </button>
            </div>
        );
    }

    if (success) {
        return (
            <div className="booking-success">
                <p>Booking created successfully! Redirecting to your bookings...</p>
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

            <div className="booking-summary">
                <h4>Booking Summary</h4>
                <p>Price per night: ${price}</p>
                <p>Total: ${bookingData.totalPrice}</p>
            </div>

            {error && <div className="error-message">{error}</div>}

            <button type="submit" disabled={loading} className="book-button">
                {loading ? 'Processing...' : 'Book Now'}
            </button>
        </form>
    );
};

export default BookingForm;