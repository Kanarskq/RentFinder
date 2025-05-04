import React from 'react';
import { useNavigate } from 'react-router-dom';
import { messageApi } from '../../api/messageApi';
import { useAuth } from '../../hooks/useAuth';

const ContactOwnerButton = ({ ownerId, propertyId, propertyTitle }) => {
    const navigate = useNavigate();
    const { isAuthenticated, login } = useAuth();
    const [loading, setLoading] = React.useState(false);

    const handleContactOwner = async () => {
        if (!isAuthenticated) {
            login();
            return;
        }

        try {
            setLoading(true);
            const initialMessage = `Hello, I'm interested in your property "${propertyTitle}". Is it still available?`;

            await messageApi.sendMessage(ownerId, initialMessage, propertyId);
            navigate('/chat');
        } catch (error) {
            console.error('Error starting conversation:', error);
            alert('Could not start conversation. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <button
            className="contact-owner-button"
            onClick={handleContactOwner}
            disabled={loading}
        >
            {loading ? 'Connecting...' : 'Contact Owner'}
        </button>
    );
};

export default ContactOwnerButton;