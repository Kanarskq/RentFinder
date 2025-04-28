import React, { createContext, useState, useEffect } from 'react';
import { authApi } from '../api/authApi';

export const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [currentUser, setCurrentUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Проверка авторизации при загрузке
    useEffect(() => {
        fetchUserProfile();
    }, []);

    const fetchUserProfile = async () => {
        try {
            setLoading(true);
            console.log('Attempting to fetch user profile...');
            const { data } = await authApi.getUserProfile();

            console.log('User profile received:', data);

            if (data) {
                // Process user data
                const user = {
                    firstName: data.name ? data.name.split(' ')[0] : '',
                    lastName: data.name ? data.name.split(' ')[1] || '' : '',
                    email: data.emailAddress,
                    role: data.role,
                    userId: data.userId,
                    createdAt: new Date().toISOString(),
                    profileImage: data.profileImage
                };

                setCurrentUser(user);

                // Save token if it came in the response
                if (data.accessToken) {
                    console.log('Saving new token from profile response');
                    localStorage.setItem('token', data.accessToken);
                }
            }

            setError(null);
        } catch (err) {
            console.error('Failed to fetch user profile:', err);
            console.error('Error details:', err.response ? err.response.data : 'No response data');
            setCurrentUser(null);
            setError('Failed to fetch user profile. Please log in again.');
        } finally {
            setLoading(false);
        }
    };

    const logout = async () => {
        try {
            localStorage.removeItem('token');
            setCurrentUser(null);
            await authApi.logout();
        } catch (err) {
            console.error('Logout error:', err);
        }
    };

    const value = {
        currentUser,
        loading,
        error,
        logout,
        isAuthenticated: !!currentUser,
        refreshUserProfile: fetchUserProfile
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};