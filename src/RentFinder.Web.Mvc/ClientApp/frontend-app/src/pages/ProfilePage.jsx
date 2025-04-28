import React from 'react';
import { useAuth } from '../hooks/useAuth';

const ProfilePage = () => {
    const { currentUser } = useAuth();

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
                        {currentUser.firstName?.charAt(0)}{currentUser.lastName?.charAt(0)}
                    </div>
                    <h2>{currentUser.firstName} {currentUser.lastName}</h2>
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
                </div>
            </div>
        </div>
    );
};

export default ProfilePage;