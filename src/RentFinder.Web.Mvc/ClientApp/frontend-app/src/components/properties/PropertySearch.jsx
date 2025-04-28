import React, { useState } from 'react';

const PropertySearch = ({ onSearch }) => {
    const [searchParams, setSearchParams] = useState({
        location: '',
        minPrice: '',
        maxPrice: '',
        bedrooms: '',
        bathrooms: '',
        minArea: '',
        propertyType: ''
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setSearchParams(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Filter out empty values
        const filteredParams = Object.entries(searchParams)
            .reduce((acc, [key, value]) => {
                if (value !== '') {
                    acc[key] = value;
                }
                return acc;
            }, {});

        onSearch(filteredParams);
    };

    return (
        <form className="property-search-form" onSubmit={handleSubmit}>
            <div className="form-row">
                <div className="form-group">
                    <label htmlFor="location">Location</label>
                    <input
                        type="text"
                        id="location"
                        name="location"
                        value={searchParams.location}
                        onChange={handleChange}
                        placeholder="City, neighborhood, or address"
                    />
                </div>
            </div>

            <div className="form-row">
                <div className="form-group">
                    <label htmlFor="minPrice">Min Price</label>
                    <input
                        type="number"
                        id="minPrice"
                        name="minPrice"
                        value={searchParams.minPrice}
                        onChange={handleChange}
                        placeholder="Min Price"
                    />
                </div>
                <div className="form-group">
                    <label htmlFor="maxPrice">Max Price</label>
                    <input
                        type="number"
                        id="maxPrice"
                        name="maxPrice"
                        value={searchParams.maxPrice}
                        onChange={handleChange}
                        placeholder="Max Price"
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
                        <option value="1">1+</option>
                        <option value="2">2+</option>
                        <option value="3">3+</option>
                        <option value="4">4+</option>
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
                        <option value="1">1+</option>
                        <option value="2">2+</option>
                        <option value="3">3+</option>
                    </select>
                </div>
            </div>

            <div className="form-row">
                <div className="form-group">
                    <label htmlFor="minArea">Min Area (sq ft)</label>
                    <input
                        type="number"
                        id="minArea"
                        name="minArea"
                        value={searchParams.minArea}
                        onChange={handleChange}
                        placeholder="Min Area"
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
                        <option value="Apartment">Apartment</option>
                        <option value="House">House</option>
                        <option value="Condo">Condo</option>
                        <option value="Villa">Villa</option>
                    </select>
                </div>
            </div>

            <button type="submit" className="search-button">Search</button>
        </form>
    );
};

export default PropertySearch;