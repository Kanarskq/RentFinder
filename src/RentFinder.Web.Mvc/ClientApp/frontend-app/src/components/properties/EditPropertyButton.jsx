import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Edit } from 'lucide-react';
import '../../styles/PropertyButtonStyles.css';

const EditPropertyButton = ({ propertyId, className }) => {
    const navigate = useNavigate();

    const handleEditProperty = () => {
        navigate(`/property/${propertyId}/edit`);
    };

    return (
        <button
            onClick={handleEditProperty}
            className={`
                property-action-btn 
                edit-property-btn 
                btn-full-width
                btn-inline' 
                ${className || ''}
            `}
            aria-label="Edit property"
        >
            <Edit size={20} />
            <span>Edit Property</span>
        </button>
    );
};

export default EditPropertyButton