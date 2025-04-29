import React, { useEffect, useState, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { propertyApi } from '../api/propertyApi';
import { AuthContext } from '../context/AuthContext';
import '../styles/PropertyStyles.css';
import PropertyReviews from '../components/reviews/PropertyReviews';

const PropertyDetailPage = () => {
    const { id } = useParams();
    const [property, setProperty] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const { isAuthenticated } = useContext(AuthContext);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProperty = async () => {
            try {
                const response = await propertyApi.getPropertyById(id);
                setProperty(response.data);
            } catch (err) {
                console.error('Error fetching property details:', err);
                setError('Failed to load property details');
            } finally {
                setLoading(false);
            }
        };

        if (id) {
            fetchProperty();
        }
    }, [id]);

    const handleBookNow = () => {
        if (!isAuthenticated) {
            navigate('/login', { state: { redirectTo: `/bookings/new/${id}` } });
        } else {
            navigate(`/bookings/new/${id}`);
        }
    };

    const handleFindSimilar = () => {
        const searchParams = {
            price: property.price,
            latitude: property.latitude,
            longitude: property.longitude,
            squareFootage: property.squareFootage,
            bedrooms: property.bedrooms,
            bathrooms: property.bathrooms,
            yearBuilt: property.yearBuilt,
            hasBalcony: property.hasBalcony,
            hasParking: property.hasParking,
            petsAllowed: property.petsAllowed,
            propertyType: property.propertyType,
            maxResults: 12
        };

        navigate('/similar-properties', { state: { searchParams } });
    };

    if (loading) {
        return <div className="loading">Loading property details...</div>;
    }

    if (error) {
        return <div className="error">Error: {error}</div>;
    }

    if (!property) {
        return <div className="no-results">Property not found</div>;
    }

    const imageUrl = `${process.env.REACT_APP_API_URL || 'https://localhost:7000'}/api/property/${id}/image`;

    return (
        <div className="property-detail-page">
            <div className="property-header">
                <h1>{property.title}</h1>
                <div className="property-price-header">${property.price} per night</div>
            </div>

            <div className="property-detail-container">
                <div className="property-image-gallery">
                    <img
                        src={imageUrl}
                        alt={property.title}
                        className="property-main-image"
                        onError={(e) => {
                            e.target.onerror = null;
                            e.target.src = '/placeholder-property.jpg';
                        }}
                    />
                </div>

                <div className="property-info-container">
                    <div className="property-main-details">
                        <h2>Property Details</h2>
                        <div className="property-specs">
                            <div className="spec-item">
                                <span className="spec-label">Bedrooms:</span>
                                <span className="spec-value">{property.bedrooms}</span>
                            </div>
                            <div className="spec-item">
                                <span className="spec-label">Bathrooms:</span>
                                <span className="spec-value">{property.bathrooms}</span>
                            </div>
                            <div className="spec-item">
                                <span className="spec-label">Square Footage:</span>
                                <span className="spec-value">{property.squareFootage} sq ft</span>
                            </div>
                            <div className="spec-item">
                                <span className="spec-label">Property Type:</span>
                                <span className="spec-value">{property.propertyType}</span>
                            </div>
                            <div className="spec-item">
                                <span className="spec-label">Year Built:</span>
                                <span className="spec-value">{property.yearBuilt}</span>
                            </div>
                        </div>

                        <div className="property-features-list">
                            <h3>Features</h3>
                            <ul>
                                {property.hasBalcony && <li>Balcony</li>}
                                {property.hasParking && <li>Parking available</li>}
                                {property.petsAllowed && <li>Pets allowed</li>}
                            </ul>
                        </div>

                        <div className="property-description">
                            <h3>Description</h3>
                            <p>{property.description}</p>
                        </div>

                        <div className="property-location">
                            <h3>Location</h3>
                            <p>Latitude: {property.latitude}, Longitude: {property.longitude}</p>
                            {/* Add a map component here */}
                        </div>

                        <div className="property-actions">
                            <button
                                className="book-now-button"
                                onClick={handleBookNow}
                            >
                                Book Now
                            </button>
                            <button
                                className="find-similar-button"
                                onClick={handleFindSimilar}
                            >
                                Find Similar Properties
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <div className="property-reviews-container">
                <PropertyReviews />
            </div>
        </div>
    );
};

export default PropertyDetailPage;