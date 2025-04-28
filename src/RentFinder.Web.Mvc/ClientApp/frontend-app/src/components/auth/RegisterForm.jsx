import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

const RegisterForm = () => {
    const [userData, setUserData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: ''
    });
    const [error, setError] = useState('');
    const { register } = useAuth();
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setUserData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        if (userData.password !== userData.confirmPassword) {
            setError("Passwords don't match");
            return;
        }

        const result = await register(userData);
        if (result.success) {
            navigate('/profile');
        } else {
            setError(result.error);
        }
    };

    return (
        <div className="auth-form">
            <h2>Register</h2>
            {error && <div className="error-message">{error}</div>}

            <form onSubmit={handleSubmit}>
                <div className="form-row">
                    <div className="form-group">
                        <label htmlFor="firstName">First Name</label>
                        <input
                            type="text"
                            id="firstName"
                            name="firstName"
                            value={userData.firstName}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label htmlFor="lastName">Last Name</label>
                        <input
                            type="text"
                            id="lastName"
                            name="lastName"
                            value={userData.lastName}
                            onChange={handleChange}
                            required
                        />
                    </div>
                </div>

                <div className="form-group">
                    <label htmlFor="email">Email</label>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        value={userData.email}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="password">Password</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        value={userData.password}
                        onChange={handleChange}
                        required
                        minLength="6"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="confirmPassword">Confirm Password</label>
                    <input
                        type="password"
                        id="confirmPassword"
                        name="confirmPassword"
                        value={userData.confirmPassword}
                        onChange={handleChange}
                        required
                        minLength="6"
                    />
                </div>

                <button type="submit" className="submit-button">Register</button>
            </form>

            <div className="auth-links">
                <p>Already have an account? <a href="/login">Login</a></p>
            </div>
        </div>
    );
};

export default RegisterForm;