import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { authApi } from '../../api/authApi';

const AuthCallback = () => {
    const [error, setError] = useState(null);
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        const handleCallback = async () => {
            try {
                const userData = await authApi.getProfile();
                console.log("Successfully authenticated user:", userData);

                const returnUrl = sessionStorage.getItem('returnUrl') || '/';
                sessionStorage.removeItem('returnUrl'); 

                navigate(returnUrl);
            } catch (err) {
                console.error('Error during authentication callback:', err);
                setError('Authentication failed. Please try again.');
                setTimeout(() => navigate('/login'), 3000);
            }
        };

        handleCallback();
    }, [navigate, location]);

    if (error) {
        return (
            <div className="auth-callback">
                <p className="error">{error}</p>
                <p>Redirecting to login page...</p>
            </div>
        );
    }

    return (
        <div className="auth-callback">
            <p>Authentication in progress...</p>
        </div>
    );
};

export default AuthCallback;