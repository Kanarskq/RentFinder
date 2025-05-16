import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Edit } from 'lucide-react';

const EditPropertyButton = ({ propertyId, className }) => {
    const navigate = useNavigate();

    const handleEditProperty = () => {
        navigate(`/property/${propertyId}/edit`);
    };

    return (
        <button
            onClick={handleEditProperty}
            className={`flex items-center gap-2 bg-gray-600 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded-lg transition-colors ${className || ''}`}
            aria-label="Edit property"
        >
            <Edit size={20} />
            <span>Edit Property</span>
        </button>
    );
};

export default EditPropertyButton