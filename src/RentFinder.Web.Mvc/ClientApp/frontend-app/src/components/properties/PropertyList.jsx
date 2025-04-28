import React from 'react';
import PropertyCard from './PropertyCard';

const PropertyList = ({ properties, loading, error }) => {
    if (loading) {
        return <div className="loading">Loading properties...</div>;
    }

    if (error) {
        return <div className="error">Error: {error}</div>;
    }

    if (!properties || properties.length === 0) {
        return <div className="no-results">No properties found</div>;
    }

    return (
        <div className="property-list">
            {properties.map(property => (
                <PropertyCard key={property.id} property={property} />
            ))}
        </div>
    );
};

export default PropertyList;