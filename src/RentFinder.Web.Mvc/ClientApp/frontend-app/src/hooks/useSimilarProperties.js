import { useState } from 'react';
import { propertyApi } from '../api/propertyApi';

export const useSimilarProperties = () => {
    const [similarProperties, setSimilarProperties] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const fetchSimilarProperties = async (searchParams) => {
        setLoading(true);
        setError(null);

        try {
            const response = await propertyApi.getSimilarProperties(searchParams);
            setSimilarProperties(response.data);
        } catch (err) {
            setError(err.response?.data?.message || 'Failed to fetch similar properties');
            console.error('Error fetching similar properties:', err);
        } finally {
            setLoading(false);
        }
    };

    return {
        similarProperties,
        loading,
        error,
        fetchSimilarProperties
    };
};