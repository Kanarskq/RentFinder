import React from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Edit } from 'lucide-react';

const AddPropertyButton = ({ className }) => {
    const navigate = useNavigate();

    const handleAddProperty = () => {
        navigate('/property/create');
    };

    return (
        <button
            onClick={handleAddProperty}
            className={`flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors ${className || ''}`}
            aria-label="Add property"
        >
            <Plus size={20} />
            <span>Add Property</span>
        </button>
    );
};

export default AddPropertyButton