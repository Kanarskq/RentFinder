import apiClient from './apiClient';

export const authApi = {
    getUserProfile: () => {
        console.log('Fetching user profile...');
        return apiClient.get('/auth/profile');
    },

    logout: () => {
        console.log('Logging out...');
        localStorage.removeItem('token');
        window.location.href = 'https://localhost:7224/auth/logout';
    },

    loginWithAuth0: () => {
        console.log('Initiating Auth0 login...');
        const returnUrl = window.location.origin + '/auth/callback';
        window.location.href = 'https://localhost:7224/auth/authorize?returnUrl=' +
            encodeURIComponent(returnUrl);
    }
};