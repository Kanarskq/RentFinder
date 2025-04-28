import React from 'react';
import { Link } from 'react-router-dom';

const PropertyCard = ({ property }) => {
    return (
        <div className="property-card">
            <img
                src={property.imageUrl || 'https://via.placeholder.com/300x200'}
                alt={property.title}
                className="property-image"
            />
            <div className="property-info">
                <h3>{property.title}</h3>
                <p className="property-price">${property.price}/night</p>
                <p className="property-address">{property.address}</p>
                <div className="property-details">
                    <span>{property.bedrooms} beds</span> •
                    <span>{property.bathrooms} baths</span> •
                    <span>{property.area} sq ft</span>
                </div>
                <Link to={`/properties/${property.id}`} className="view-details-btn">
                    View Details
                </Link>
            </div>
        </div>
    );
};

export default PropertyCard;