import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Edit } from 'lucide-react';
import '../../styles/PropertyButtonStyles.css';

const AddPropertyButton = ({ className }) => {
    const navigate = useNavigate();

    const handleAddProperty = () => {
        navigate('/property/create');
    };

    return (
        <button
            onClick={handleAddProperty}
            className={`
                property-action-btn 
                add-property-btn 
                floating-add-btn
                ${className || ''}
            `}
            aria-label="Add property"
        >
            <Plus size={20} />
            <span>Add Property</span>
        </button>
    );
};

export default AddPropertyButton