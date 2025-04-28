import axios from 'axios';

const BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7224';

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
            console.log('Using token:', token);
            config.headers.Authorization = `Bearer ${token}`;
        } else {
            console.log('No token found');
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
        if (error.response && error.response.status === 401) {
            // Redirect to login or refresh token
            window.location.href = `${BASE_URL}/auth/login?returnUrl=${window.location.href}`;
        }
        return Promise.reject(error);
    }
);

export default apiClient;