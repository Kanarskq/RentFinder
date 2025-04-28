﻿import React from 'react';
import LoginForm from '../components/auth/LoginForm';

const LoginPage = () => {
    return (
        <div className="auth-page">
            <div className="auth-container">
                <LoginForm />
            </div>
        </div>
    );
};

export default LoginPage;