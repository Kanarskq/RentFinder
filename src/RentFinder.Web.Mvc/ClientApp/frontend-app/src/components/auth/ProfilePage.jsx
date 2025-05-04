import React from 'react';
import { useAuth } from '../hooks/useAuth';

const ProfilePage = () => {
    const { currentUser, logout } = useAuth();

    if (!currentUser) {
        return (
            <div className="profile-page">
                <p>Please log in to view your profile</p>
            </div>
        );
    }

    return (
        <div className="profile-page">
            <h1>Your Profile</h1>

            <div className="profile-info">
                <div className="profile-header">
                    <div className="avatar">
                        {currentUser.name?.charAt(0)}
                    </div>
                    <h2>{currentUser.name}</h2>
                    <p>{currentUser.email}</p>
                </div>

                <div className="profile-details">
                    <h3>Account Details</h3>
                    <div className="detail-item">
                        <span className="detail-label">Member Since:</span>
                        <span className="detail-value">
                            {new Date(currentUser.createdAt).toLocaleDateString()}
                        </span>
                    </div>

                    <h3>Contact Information</h3>
                    <div className="detail-item">
                        <span className="detail-label">Phone:</span>
                        <span className="detail-value">
                            {currentUser.phone || 'Not provided'}
                        </span>
                    </div>
                </div>
            </div>

            <div className="profile-actions">
                <button className="edit-profile-button">Edit Profile</button>
                <button className="logout-button" onClick={logout}>Logout</button>
            </div>
        </div>
    );
};

export default ProfilePage;