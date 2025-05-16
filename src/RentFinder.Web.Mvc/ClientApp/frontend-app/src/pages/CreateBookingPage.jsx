import React from 'react';
import { useParams } from 'react-router-dom';
import BookingForm from '../components/bookings/BookingForm';
import { useProperties } from '../hooks/useProperties';
import '../styles/BookingStyles.css';

const CreateBookingPage = () => {
    const { propertyId } = useParams();
    const { property } = useProperties(propertyId);

    return (
        <div className="create-booking-page">
            {property && (
                <BookingForm propertyId={property.id} price={property.price} />
            )}
        </div>
    );
};

export default CreateBookingPage;