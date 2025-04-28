import React from 'react';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../../api/authApi';

const LoginForm = () => {
    const navigate = useNavigate();

    const handleExternalLogin = () => {
        authApi.loginWithAuth0();
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