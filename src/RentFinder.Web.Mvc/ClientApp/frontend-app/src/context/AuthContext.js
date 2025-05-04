import React, { createContext, useState, useEffect } from 'react';
import { authApi } from '../api/authApi';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [currentUser, setCurrentUser] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const initAuth = async () => {
            try {
                if (authApi.isAuthenticated()) {
                    const profile = await authApi.getProfile();
                    setCurrentUser(profile);
                    setIsAuthenticated(true);
                }
            } catch (error) {
                console.error('Authentication error:', error);
                localStorage.removeItem('token');
                localStorage.removeItem('id_token');
            } finally {
                setLoading(false);
            }
        };

        initAuth();
    }, []);

    const logout = () => {
        authApi.logout();
        setCurrentUser(null);
        setIsAuthenticated(false);
    };

    const login = () => {
        authApi.loginWithAuth0();
    };

    const value = {
        currentUser,
        isAuthenticated,
        loading,
        login,
        logout
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};