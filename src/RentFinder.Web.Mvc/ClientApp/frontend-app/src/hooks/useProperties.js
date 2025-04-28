import { useState, useEffect } from 'react';
import { propertyApi } from '../api/propertyApi';

export const useProperties = (id) => {
    const [properties, setProperties] = useState([]);
    const [property, setProperty] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const fetchProperties = async (searchParams) => {
        setLoading(true);
        setError(null);
        try {
            const response = await propertyApi.searchProperties(searchParams);
            setProperties(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch properties');
        } finally {
            setLoading(false);
        }
    };

    const fetchPropertyById = async (id) => {
        setLoading(true);
        setError(null);
        try {
            const response = await propertyApi.getPropertyById(id);
            setProperty(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch property');
        } finally {
            setLoading(false);
        }
    };

    const fetchRecommendedProperties = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await propertyApi.getRecommendedProperties();
            setProperties(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch recommended properties');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) {
            fetchPropertyById(id);
        }
    }, [id]);

    return {
        properties,
        property,
        loading,
        error,
        fetchProperties,
        fetchRecommendedProperties
    };
};