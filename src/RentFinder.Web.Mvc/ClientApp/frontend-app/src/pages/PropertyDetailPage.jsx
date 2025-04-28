import React from 'react';
import { useParams } from 'react-router-dom';
import PropertyDetail from '../components/properties/PropertyDetail';

const PropertyDetailPage = () => {
    const { id } = useParams();

    return (
        <div className="property-detail-page">
            <PropertyDetail id={id} />
        </div>
    );
};

export default PropertyDetailPage;