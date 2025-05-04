import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

const LoginForm = () => {
    const { login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    const from = location.state?.from?.pathname || '/';
    const searchParams = new URLSearchParams(location.search);
    const returnUrl = searchParams.get('returnUrl') || from;

    const handleExternalLogin = () => {
        if (returnUrl) {
            sessionStorage.setItem('returnUrl', returnUrl);
        }
        login();
    };

    return (
        <div className="auth-form">
            <h2>Login</h2>

            <div className="auth-external">
                <button
                    onClick={handleExternalLogin}
                    className="external-login-button"
                >
                    Log in with Auth0
                </button>
            </div>
        </div>
    );
};

export default LoginForm;