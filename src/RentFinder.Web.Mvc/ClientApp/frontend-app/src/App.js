import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import MainLayout from './components/layout/MainLayout';
import SimilarPropertiesPage from './pages/SimilarPropertiesPage';
import PropertyDetailPage from './pages/PropertyDetailPage';
import BookingsPage from './pages/BookingsPage';
import CreateBookingPage from './pages/CreateBookingPage';
import LoginPage from './pages/LoginPage';
import ProfilePage from './pages/ProfilePage';
import MessagesPage from './pages/MessagesPage';
import AuthCallback from './components/auth/AuthCallback';
import { AuthProvider } from './context/AuthContext';
import PropertyFormPage from './pages/PropertyFormPage';
import PrivateRoute from './components/auth/PrivateRoute';
import BookingDetailPage from './pages/BookingsPage';
import './App.css';
import './styles/MapStyles.css';
import './styles/MessageStyles.css';

function App() {

    return (
        <Router>
            <AuthProvider>
                <Routes>
                    <Route path="/" element={<MainLayout />}>
                        <Route index element={<SimilarPropertiesPage />} />
                        <Route path="similar-properties" element={<SimilarPropertiesPage />} />
                        <Route path="property/:id" element={<PropertyDetailPage />} />
                        <Route path="property/create" element={
                            <PrivateRoute>
                                <PropertyFormPage />
                            </PrivateRoute>
                        } />
                        <Route path="property/:id/edit" element={
                            <PrivateRoute>
                                <PropertyFormPage />
                            </PrivateRoute>
                        } />
                        <Route path="bookings" element={
                            <PrivateRoute>
                                <BookingsPage />
                            </PrivateRoute>
                        } />
                        <Route path="bookings/:id" element={
                            <PrivateRoute>
                                <BookingDetailPage />
                            </PrivateRoute>
                        } />
                        <Route path="bookings/new/:propertyId" element={
                            <PrivateRoute>
                                <CreateBookingPage />
                            </PrivateRoute>
                        } />
                        <Route path="chat" element={
                            <PrivateRoute>
                                <MessagesPage />
                            </PrivateRoute>
                        } />
                        <Route path="login" element={<LoginPage />} />
                        <Route path="auth/callback" element={<AuthCallback />} />
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