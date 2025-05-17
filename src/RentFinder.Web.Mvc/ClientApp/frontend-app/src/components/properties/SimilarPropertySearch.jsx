import React, { useState } from 'react';
import MapLocationSelector from '../maps/MapLocationSelector';

const SimilarPropertySearch = ({ onSearch }) => {
    const [searchParams, setSearchParams] = useState({
        price: '',
        latitude: '',
        longitude: '',
        squareFootage: '',
        bedrooms: '',
        bathrooms: '',
        yearBuilt: '',
        hasBalcony: false,
        hasParking: false,
        petsAllowed: false,
        propertyType: '',
        maxResults: 12
    });

    const handleLocationSelect = (location) => {
        if (location && typeof location === 'object' && location.lat && location.lng) {
            setSearchParams(prev => ({
                ...prev,
                latitude: location.lat.toString(),
                longitude: location.lng.toString()
            }));
        }
    };

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setSearchParams(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        const formattedParams = {
            ...searchParams,
            price: searchParams.price ? parseFloat(searchParams.price) : null,
            latitude: searchParams.latitude ? parseFloat(searchParams.latitude) : null,
            longitude: searchParams.longitude ? parseFloat(searchParams.longitude) : null,
            squareFootage: searchParams.squareFootage ? parseFloat(searchParams.squareFootage) : null,
            bedrooms: searchParams.bedrooms ? parseInt(searchParams.bedrooms) : null,
            bathrooms: searchParams.bathrooms ? parseInt(searchParams.bathrooms) : null,
            yearBuilt: searchParams.yearBuilt ? parseInt(searchParams.yearBuilt) : null,
            maxResults: 12
        };

        onSearch(formattedParams);
    };

    const initialLocation = searchParams.latitude && searchParams.longitude
        ? {
            lat: parseFloat(searchParams.latitude),
            lng: parseFloat(searchParams.longitude)
        }
        : null;

    return (
        <form className="similar-property-search-form compact-form" onSubmit={handleSubmit}>
            <h2>Find Properties</h2>

            <div className="form-row">
                <div className="form-group">
                    <label htmlFor="price">Price</label>
                    <input
                        type="number"
                        id="price"
                        name="price"
                        value={searchParams.price}
                        onChange={handleChange}
                        placeholder="Price per night"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="squareFootage">Square Footage</label>
                    <input
                        type="number"
                        id="squareFootage"
                        name="squareFootage"
                        value={searchParams.squareFootage}
                        onChange={handleChange}
                        placeholder="Square Footage"
                    />
                </div>
            </div>

            <div className="form-row">
                <div className="form-group">
                    <label htmlFor="bedrooms">Bedrooms</label>
                    <select
                        id="bedrooms"
                        name="bedrooms"
                        value={searchParams.bedrooms}
                        onChange={handleChange}
                    >
                        <option value="">Any</option>
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                        <option value="4">4</option>
                    </select>
                </div>
                <div className="form-group">
                    <label htmlFor="bathrooms">Bathrooms</label>
                    <select
                        id="bathrooms"
                        name="bathrooms"
                        value={searchParams.bathrooms}
                        onChange={handleChange}
                    >
                        <option value="">Any</option>
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                        <option value="4">4</option>
                    </select>
                </div>
            </div>

            <div className="form-row">
                <div className="form-group">
                    <label htmlFor="yearBuilt">Year Built</label>
                    <input
                        type="number"
                        id="yearBuilt"
                        name="yearBuilt"
                        value={searchParams.yearBuilt}
                        onChange={handleChange}
                        placeholder="Year Built"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="propertyType">Property Type</label>
                    <select
                        id="propertyType"
                        name="propertyType"
                        value={searchParams.propertyType}
                        onChange={handleChange}
                    >
                        <option value="">Any</option>
                        <option value="apartment">Apartment</option>
                        <option value="house">House</option>
                        <option value="condo">Condo</option>
                        <option value="villa">Villa</option>
                        <option value="studio">Studio</option>
                    </select>
                </div>
            </div>

            <div className="form-row location-input">
                <div className="location-coordinates">
                    <div className="form-group half-width">
                        <label htmlFor="latitude">Latitude</label>
                        <input
                            type="number"
                            id="latitude"
                            name="latitude"
                            value={searchParams.latitude}
                            onChange={handleChange}
                            step="0.000001"
                            placeholder="Latitude"
                        />
                    </div>
                    <div className="form-group half-width">
                        <label htmlFor="longitude">Longitude</label>
                        <input
                            type="number"
                            id="longitude"
                            name="longitude"
                            onChange={handleChange}
                            step="0.000001"
                            placeholder="Longitude"
                            value={searchParams.longitude}
                        />
                    </div>
                </div>
                <div className="map-selector-container">
                    <MapLocationSelector
                        onLocationSelect={handleLocationSelect}
                        initialLocation={initialLocation}
                    />
                </div>
            </div>

            <div className="form-row checkboxes">
                <div className="form-group checkbox">
                    <label>
                        <input
                            type="checkbox"
                            name="hasBalcony"
                            checked={searchParams.hasBalcony}
                            onChange={handleChange}
                        />
                        Has Balcony
                    </label>
                </div>
                <div className="form-group checkbox">
                    <label>
                        <input
                            type="checkbox"
                            name="hasParking"
                            checked={searchParams.hasParking}
                            onChange={handleChange}
                        />
                        Has Parking
                    </label>
                </div>
                <div className="form-group checkbox">
                    <label>
                        <input
                            type="checkbox"
                            name="petsAllowed"
                            checked={searchParams.petsAllowed}
                            onChange={handleChange}
                        />
                        Pets Allowed
                    </label>
                </div>
            </div>

            <button type="submit" className="search-button">Find Properties</button>
        </form>
    );
};

export default SimilarPropertySearch;