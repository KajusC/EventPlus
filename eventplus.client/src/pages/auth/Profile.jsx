import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { getUserProfile, updateUserProfile } from '../../services/authService';
import { useNotification } from '../../context/NotificationContext';
import './Auth.css';

const Profile = () => {
  const { currentUser, token } = useAuth();
  const { showSuccess, showError } = useNotification();
  const [profileData, setProfileData] = useState({
    name: '',
    surname: '',
    username: ''
  });
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (currentUser) {
      setProfileData({
        name: currentUser.name || '',
        surname: currentUser.surname || '',
        username: currentUser.username || ''
      });
    }
  }, [currentUser]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setProfileData({
      ...profileData,
      [name]: value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      await updateUserProfile(profileData, token);
      showSuccess('Profile updated successfully!');
      setIsEditing(false);
    } catch (err) {
      console.error('Update profile error:', err);
      showError('Failed to update profile. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  if (!currentUser) {
    return <div className="auth-container">Please log in to view your profile.</div>;
  }

  return (
    <div className="auth-container">
      <div className="auth-form-container">
        <h2>Your Profile</h2>
        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              type="text"
              id="username"
              name="username"
              value={profileData.username}
              onChange={handleChange}
              readOnly={!isEditing}
              disabled={!isEditing}
            />
          </div>
          <div className="form-group">
            <label htmlFor="name">First Name</label>
            <input
              type="text"
              id="name"
              name="name"
              value={profileData.name}
              onChange={handleChange}
              readOnly={!isEditing}
              disabled={!isEditing}
            />
          </div>
          <div className="form-group">
            <label htmlFor="surname">Last Name</label>
            <input
              type="text"
              id="surname"
              name="surname"
              value={profileData.surname}
              onChange={handleChange}
              readOnly={!isEditing}
              disabled={!isEditing}
            />
          </div>
          <div className="profile-role">
            <strong>Role:</strong> {currentUser.role}
          </div>
          
          {isEditing ? (
            <div className="button-group">
              <button type="submit" disabled={loading} className="auth-button">
                {loading ? 'Saving...' : 'Save Changes'}
              </button>
              <button 
                type="button" 
                className="auth-button secondary"
                onClick={() => setIsEditing(false)}
              >
                Cancel
              </button>
            </div>
          ) : (
            <button 
              type="button" 
              className="auth-button"
              onClick={() => setIsEditing(true)}
            >
              Edit Profile
            </button>
          )}
        </form>
      </div>
    </div>
  );
};

export default Profile;