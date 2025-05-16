import React, { useState } from 'react';
import { useAuth } from '../../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import GooglePayButton from './GooglePayButton';
import { bookingApi } from '../../api/bookingApi';

const BookingForm = ({ propertyId, price, landlordId }) => {
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

        setBookingData(prev => {
            const newData = {
                ...prev,
                [name]: value
            };

            if (name === 'startDate' || name === 'endDate') {
                const startDate = name === 'startDate' ? value : prev.startDate;
                const endDate = name === 'endDate' ? value : prev.endDate;

                if (startDate && endDate) {
                    const start = new Date(startDate);
                    const end = new Date(endDate);

                    if (!isNaN(start.getTime()) && !isNaN(end.getTime())) {
                        const days = Math.max(1, Math.ceil((end - start) / (1000 * 60 * 60 * 24)));
                        const total = days * price;
                        newData.totalPrice = total;
                    }
                }
            }

            return newData;
        });
    };

    const calculateTotal = () => {
        if (!bookingData.startDate || !bookingData.endDate) return 0;

        const start = new Date(bookingData.startDate);
        const end = new Date(bookingData.endDate);

        if (isNaN(start.getTime()) || isNaN(end.getTime())) return 0;

        const days = Math.max(1, Math.ceil((end - start) / (1000 * 60 * 60 * 24)));
        const total = days * price;

        return total;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            await createBooking('Pending');
        } catch (err) {
            setError('Failed to create booking. Please try again.');
            setLoading(false);
        }
    };

    const handlePaymentSuccess = (paymentResult) => {
        setLoading(true);
        createBooking('Confirmed', paymentResult.TransactionId);
    };

    const handlePaymentError = (error) => {
        setError(`Payment failed: ${error}`);
    };

    const createBooking = async (status, transactionId = null) => {
        try {
            const formattedData = {
                propertyId: parseInt(propertyId),
                userId: parseInt(currentUser?.id || bookingData.userId),
                startDate: new Date(bookingData.startDate).toISOString(),
                endDate: new Date(bookingData.endDate).toISOString(),
                totalPrice: bookingData.totalPrice,
                createdAt: new Date().toISOString(),
                status: status
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

            {bookingData.totalPrice > 0 && (
                <div className="payment-options">
                    <GooglePayButton
                        amount={bookingData.totalPrice}
                        renterId={currentUser?.id}
                        landlordId={landlordId}
                        description={`Booking for property ${propertyId}`}
                        onPaymentSuccess={handlePaymentSuccess}
                        onPaymentError={handlePaymentError}
                        disabled={!bookingData.startDate || !bookingData.endDate}
                    />
                    <div className="payment-divider">or</div>
                    <button
                        type="submit"
                        disabled={loading}
                        className="book-button"
                    >
                        {loading ? 'Processing...' : 'Book Now (Pay Later)'}
                    </button>
                </div>
            )}

            {(!bookingData.totalPrice || bookingData.totalPrice <= 0) && (
                <button type="submit" disabled={loading} className="book-button">
                    {loading ? 'Processing...' : 'Book Now'}
                </button>
            )}
        </form>
    );
};

export default BookingForm;