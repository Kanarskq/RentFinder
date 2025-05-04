import axios from 'axios';

const BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7000';

const apiClient = axios.create({
    baseURL: BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    },
    withCredentials: true
});

apiClient.interceptors.request.use(
    config => {
        const token = localStorage.getItem('token');
        if (token) {
            console.log('Adding token to request headers');
            config.headers.Authorization = `Bearer ${token}`;
        } else {
            console.log('No token found for request');
        }
        return config;
    },
    error => {
        return Promise.reject(error);
    }
);

apiClient.interceptors.response.use(
    response => response,
    error => {
        console.error('API request error:', error);

        if (error.response) {
            if (error.response.status === 401) {
                console.log('Unauthorized request detected, redirecting to login');
                localStorage.removeItem('token'); 

                const returnUrl = encodeURIComponent(window.location.pathname + window.location.search);
                window.location.href = `/login?returnUrl=${returnUrl}`;
            }

            if (error.response.status === 0 && error.message.includes('Network Error')) {
                console.error('CORS error detected');
            }
        }

        return Promise.reject(error);
    }
);

export default apiClient;