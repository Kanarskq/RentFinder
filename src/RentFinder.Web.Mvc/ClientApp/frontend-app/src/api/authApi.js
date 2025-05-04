import apiClient from './apiClient';

const API_URL = process.env.REACT_APP_API_URL || 'https://localhost:7000';

export const authApi = {
    loginWithAuth0: () => {
        console.log("Starting Auth0 login process");
        const returnUrl = `${window.location.origin}/auth/callback`;
        console.log("Using return URL:", returnUrl);

        const authUrl = `${API_URL}/auth/authorize?returnUrl=${encodeURIComponent(returnUrl)}`;
        window.location.replace(authUrl);
    },

    getProfile: async () => {
        try {
            console.log("Fetching user profile from API");
            const response = await apiClient.get('/auth/profile');
            console.log("Profile API response:", response.data);

            if (response.data.accessToken) {
                console.log("Storing access token in localStorage");
                localStorage.setItem('token', response.data.accessToken);
            } else {
                console.warn("No access token received from profile endpoint");
            }

            if (response.data.idToken) {
                console.log("Storing ID token in localStorage");
                localStorage.setItem('id_token', response.data.idToken);
            }

            return response.data;
        } catch (error) {
            console.error('Error fetching profile:', error);
            throw error;
        }
    },

    logout: () => {
        console.log("Logging out user");
        localStorage.removeItem('token');
        localStorage.removeItem('id_token');

        window.location.href = `${API_URL}/auth/logout`;
    },

    isAuthenticated: () => {
        const hasToken = !!localStorage.getItem('token');
        console.log("Checking authentication status:", hasToken);
        return hasToken;
    }
};