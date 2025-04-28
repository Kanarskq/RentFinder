import React from 'react';
import { useParams } from 'react-router-dom';
import BookingForm from '../components/bookings/BookingForm';
import { useProperties } from '../hooks/useProperties';

const CreateBookingPage = () => {
    const { propertyId } = useParams();
    const { property } = useProperties(propertyId);

    return (
        <div className="create-booking-page">
            <h1>Create Booking</h1>
            {property && (
                <BookingForm propertyId={property.id} price={property.price} />
            )}
        </div>
    );
};

export default CreateBookingPage;