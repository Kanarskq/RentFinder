import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MainLayout from './components/layout/MainLayout';
import HomePage from './pages/HomePage';
import SimilarPropertiesPage from './pages/SimilarPropertiesPage';
import PropertyDetailPage from './pages/PropertyDetailPage';
import BookingsPage from './pages/BookingsPage';
import CreateBookingPage from './pages/CreateBookingPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import ProfilePage from './pages/ProfilePage';
import Auth0Callback from './components/auth/Auth0Callback';
import { AuthProvider } from './context/AuthContext';
import PrivateRoute from './components/auth/PrivateRoute';
import './App.css';

function App() {
    return (
        <Router>
            <AuthProvider>
                <Routes>
                    <Route path="/" element={<MainLayout />}>
                        <Route index element={<HomePage />} />
                        <Route path="similar-properties" element={<SimilarPropertiesPage />} />
                        <Route path="property/:id" element={<PropertyDetailPage />} />
                        <Route path="bookings" element={
                            <PrivateRoute>
                                <BookingsPage />
                            </PrivateRoute>
                        } />
                        <Route path="bookings/new/:propertyId" element={
                            <PrivateRoute>
                                <CreateBookingPage />
                            </PrivateRoute>
                        } />
                        <Route path="login" element={<LoginPage />} />
                        <Route path="register" element={<RegisterPage />} />
                        <Route path="auth/callback" element={<Auth0Callback />} />
                        <Route path="profile" element={
                            <PrivateRoute>
                                <ProfilePage />
                            </PrivateRoute>
                        } />
                    </Route>
                </Routes>
            </AuthProvider>
        </Router>
    );
}

export default App;