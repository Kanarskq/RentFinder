import React from 'react';
import { useProperties } from '../hooks/useProperties';
import PropertyList from '../components/properties/PropertyList';
import PropertySearch from '../components/properties/PropertySearch';

const HomePage = () => {
    const { properties, loading, error, fetchRecommendedProperties } = useProperties();

    React.useEffect(() => {
        fetchRecommendedProperties();
    }, [fetchRecommendedProperties]);

    return (
        <div className="home-page">
            <section className="hero-section">
                <div className="hero-content">
                    <h1>Find Your Perfect Rental</h1>
                    <p>Discover amazing properties for your next stay</p>
                </div>
            </section>

            <section className="search-section">
                <PropertySearch onSearch={fetchRecommendedProperties} />
            </section>
        </div>
    );
};

export default HomePage;