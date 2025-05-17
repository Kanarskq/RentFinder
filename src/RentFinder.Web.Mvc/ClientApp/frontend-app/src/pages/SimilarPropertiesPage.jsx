import React, { useState, useEffect, useContext } from 'react';
import { useLocation } from 'react-router-dom';
import { propertyApi } from '../api/propertyApi';
import SimilarPropertySearch from '../components/properties/SimilarPropertySearch';
import { AuthContext } from '../context/AuthContext';
import PropertyList from '../components/properties/PropertyList';
import AddPropertyButton from '../components/properties/AddPropertyButton';

const SimilarPropertiesPage = () => {
    const [properties, setProperties] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const { isAuthenticated } = useContext(AuthContext);
    const [searched, setSearched] = useState(false);
    const location = useLocation();

    useEffect(() => {
        const searchParams = location.state?.searchParams;
        if (searchParams) {
            handleSearch(searchParams);
        }
    }, [location.state]);

    const handleSearch = (searchParams) => {
        setLoading(true);
        setError(null);
        setSearched(true);

        propertyApi.getSimilarProperties(searchParams)
            .then(response => {
                setProperties(response.data);
            })
            .catch(err => {
                console.error('Error fetching similar properties:', err);
                setError(err.response?.data?.message || 'Failed to fetch similar properties');
            })
            .finally(() => {
                setLoading(false);
            });
    };

    return (
        <div className="similar-properties-page">
            <div className="page-container property-search-page">
                <div className="container">
                    {isAuthenticated && <AddPropertyButton />}

                    <div className="search-and-results-layout">
                        <div className="compact-search-container">
                            <SimilarPropertySearch onSearch={handleSearch} />
                        </div>

                        {searched && (
                            <div className="results-container">
                                <h2 className="results-title">Search Results</h2>
                                <PropertyList properties={properties} loading={loading} error={error} />
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default SimilarPropertiesPage;