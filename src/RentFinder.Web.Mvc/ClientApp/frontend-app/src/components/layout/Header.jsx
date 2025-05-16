import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import '../../styles/ComponentStyles.css';

const Header = () => {
    const { currentUser, isAuthenticated, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    return (
        <header className="app-header">
            <div className="header-container">
                <div className="logo">
                    <Link to="/">RentFinder</Link>
                </div>

                <nav className="main-nav">
                    <ul>
                        <li><Link to="/similar-properties">Find Property</Link></li>
                        {isAuthenticated && (
                            <>
                                <li><Link to="/bookings">My Bookings</Link></li>
                                <li><Link to="/profile">Profile</Link></li>
                                <li><Link to="/chat">Messages</Link></li>
                            </>
                        )}
                    </ul>
                </nav>

                <div className="auth-actions">
                    {isAuthenticated ? (
                        <div className="user-menu">
                            <span className="user-greeting">Hi, {currentUser?.name}</span>
                            <button onClick={handleLogout} className="logout-button">Logout</button>
                        </div>
                    ) : (
                        <>
                                <Link to="/login" className="external-login-button">Login</Link>
                        </>
                    )}
                </div>
            </div>
        </header>
    );
};

export default Header;