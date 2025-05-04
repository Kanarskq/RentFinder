import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useProperties } from '../../hooks/useProperties';
import { useAuth } from '../../hooks/useAuth'; 
import BookingForm from '../bookings/BookingForm';

const PropertyDetail = () => {
    const { id } = useParams();
    const { property, loading, error } = useProperties(id);
    const navigate = useNavigate();
    const { isAuthenticated } = useAuth(); 

    if (loading) return <div>Loading property details...</div>;
    if (error) return <div>Error: {error}</div>;
    if (!property) return <div>Property not found</div>;

    return (
        <div className="property-detail">
            <div className="property-images">
                <img src={property.imageUrl} alt={property.title} className="main-image" />
                <div className="additional-images">
                    {property.additionalImageUrls?.map((img, index) => (
                        <img key={index} src={img} alt={`${property.title} ${index + 1}`} />
                    ))}
                </div>
            </div>

            <div className="property-info">
                <h1>{property.title}</h1>
                <p className="price">${property.price} per night</p>
                <p className="address">{property.address}</p>

                <div className="property-meta">
                    <span>{property.bedrooms} bedrooms</span>
                    <span>{property.bathrooms} bathrooms</span>
                    <span>{property.area} sq ft</span>
                </div>

                <div className="property-description">
                    <h3>Description</h3>
                    <p>{property.description}</p>
                </div>

                <div className="property-amenities">
                    <h3>Amenities</h3>
                    <ul>
                        {property.amenities?.map((amenity, index) => (
                            <li key={index}>{amenity}</li>
                        ))}
                    </ul>
                </div>

                <div className="property-owner">
                    <h3>Contact Owner</h3>
                    <p>Name: {property.ownerName}</p>
                    <p>Contact: {property.ownerContact}</p>
                </div>

                <div className="property-location">
                    <h3>Location</h3>
                    <p>Latitude: {property.latitude}, Longitude: {property.longitude}</p>
                </div>
            </div>

            <div className="booking-section">
                <BookingForm propertyId={property.id} price={property.price} />
            </div>
            {isAuthenticated && (
                <button
                    onClick={() => navigate(`/chat/${property.ownerId}`)}
                    className="chat-button"
                >
                    Message Owner
                </button>
            )}
        </div>
    );
};

export default PropertyDetail;