import React, { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

const Auth0Callback = () => {
    const { refreshUserProfile } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        async function handleCallback() {
            try {
                console.log("Auth0 callback processing");

                await refreshUserProfile();

                // Navigate to appropriate page
                const state = location.state;
                const redirectTo = state && state.from ? state.from : '/profile';
                navigate(redirectTo, { replace: true });
            } catch (error) {
                console.error('Error processing Auth0 callback:', error);
                navigate('/login', { replace: true });
            }
        }

        handleCallback();
    }, [refreshUserProfile, navigate, location]);

    return (
        <div className="auth0-callback">
            <h2>Processing authentication...</h2>
            <p>Please wait while we log you in.</p>
        </div>
    );
};

export default Auth0Callback;