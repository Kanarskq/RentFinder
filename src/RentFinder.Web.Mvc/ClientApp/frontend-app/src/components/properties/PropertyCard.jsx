import React from 'react';
import { Link } from 'react-router-dom';

const PropertyCard = ({ property }) => {
    const imageUrl = property.id
        ? `${process.env.REACT_APP_API_URL || 'https://localhost:7000'}/api/property/${property.id}/image`
        : '/placeholder-property.jpg';

    return (
        <div className="property-card">
            <div className="property-image-container">
                <img
                    src={imageUrl}
                    alt={property.title}
                    className="property-image"
                    onError={(e) => {
                        e.target.onerror = null;
                        e.target.src = '/placeholder-property.jpg';
                    }}
                />
            </div>
            <div className="property-info">
                <h3>{property.title}</h3>
                <p className="property-price">${property.price} per night</p>
                <p className="property-details">
                    {property.bedrooms} BR • {property.bathrooms} BA • {property.squareFootage} sq ft
                </p>
                <p className="property-type">{property.propertyType}</p>
                <div className="property-features">
                    {property.hasBalcony && <span className="feature">Balcony</span>}
                    {property.hasParking && <span className="feature">Parking</span>}
                    {property.petsAllowed && <span className="feature">Pets</span>}
                </div>
                <Link to={`/property/${property.id}`} className="view-details-button">
                    View Details
                </Link>
            </div>
        </div>
    );
};

export default PropertyCard;