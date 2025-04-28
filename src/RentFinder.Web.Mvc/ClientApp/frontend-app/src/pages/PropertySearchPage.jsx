import React from 'react';
import { useProperties } from '../hooks/useProperties';
import PropertyList from '../components/properties/PropertyList';
import PropertySearch from '../components/properties/PropertySearch';

const PropertySearchPage = () => {
    const { properties, loading, error, fetchProperties } = useProperties();

    const handleSearch = (searchParams) => {
        fetchProperties(searchParams);
    };

    return (
        <div className="property-search-page">
            <h1>Find Your Perfect Property</h1>

            <div className="search-container">
                <PropertySearch onSearch={handleSearch} />
            </div>

            <div className="results-container">
                <PropertyList properties={properties} loading={loading} error={error} />
            </div>
        </div>
    );
};

export default PropertySearchPage;