import React, { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { propertyApi } from '../api/propertyApi';
import { AuthContext } from '../context/AuthContext';
import MapLocationSelector from '../components/maps/MapLocationSelector';
import '../styles/PropertyFormStyles.css'; 

const PropertyFormPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { currentUser, isAuthenticated } = useContext(AuthContext);
    const isEditMode = !!id;

    const [formData, setFormData] = useState({
        title: '',
        description: '',
        price: '',
        bedrooms: '',
        bathrooms: '',
        squareFootage: '',
        latitude: '',
        longitude: '',
        hasBalcony: false,
        hasParking: false,
        petsAllowed: false,
        propertyType: 'Apartment',
        yearBuilt: new Date().getFullYear(),
    });

    const [loading, setLoading] = useState(isEditMode);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const [images, setImages] = useState([]);
    const [imageUploadError, setImageUploadError] = useState(null);

    useEffect(() => {
        if (isEditMode) {
            const fetchProperty = async () => {
                try {
                    const response = await propertyApi.getPropertyById(id);
                    setFormData(response.data);
                } catch (err) {
                    console.error('Error fetching property:', err);
                    setError('Failed to load property data');
                } finally {
                    setLoading(false);
                }
            };

            fetchProperty();
        }
    }, [id, isEditMode]);

    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login', { state: { returnUrl: `/property/${isEditMode ? `${id}/edit` : 'create'}` } });
        }
    }, [isAuthenticated, navigate, id, isEditMode]);

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
    };

    const handleLocationSelect = (location) => {
        setFormData(prev => ({
            ...prev,
            latitude: location.lat,
            longitude: location.lng
        }));
    };

    const handleImageUpload = (e) => {
        setImageUploadError(null);
        const files = Array.from(e.target.files);

        const maxSize = 5 * 1024 * 1024;
        const allowedTypes = ['image/jpeg', 'image/png', 'image/gif'];

        const invalidFiles = files.filter(file =>
            file.size > maxSize || !allowedTypes.includes(file.type)
        );

        if (invalidFiles.length > 0) {
            setImageUploadError('Some files were rejected. Images must be JPG, PNG, or GIF and less than 5MB.');
            return;
        }

        const newImages = files.map(file => ({
            file,
            preview: URL.createObjectURL(file),
            caption: ''
        }));

        setImages(prev => [...prev, ...newImages]);
    };

    const handleRemoveImage = (indexToRemove) => {
        setImages(prev => {
            const updatedImages = prev.filter((_, index) => index !== indexToRemove);
            URL.revokeObjectURL(prev[indexToRemove].preview);
            return updatedImages;
        });
    };

    const handleCaptionChange = (index, caption) => {
        setImages(prev => {
            const updatedImages = [...prev];
            updatedImages[index].caption = caption;
            return updatedImages;
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSubmitting(true);
        setError(null);

        try {
            const propertyData = {
                ...formData,
                ownerId: currentUser.id,
                price: parseFloat(formData.price),
                bedrooms: parseInt(formData.bedrooms),
                bathrooms: parseInt(formData.bathrooms),
                squareFootage: parseInt(formData.squareFootage),
                latitude: parseFloat(formData.latitude),
                longitude: parseFloat(formData.longitude),
                yearBuilt: parseInt(formData.yearBuilt)
            };

            let propertyId;

            if (isEditMode) {
                await propertyApi.updateProperty(id, propertyData);
                propertyId = id;
            } else {
                const response = await propertyApi.createProperty(propertyData);
                propertyId = response.data.propertyId;
            }

            if (images.length > 0) {
                for (const image of images) {
                    const imageFormData = new FormData();
                    imageFormData.append('Image', image.file);
                    imageFormData.append('Caption', image.caption || '');

                    await propertyApi.addPropertyImage(propertyId, imageFormData);
                }
            }

            navigate(`/property/${propertyId}`);
        } catch (err) {
            console.error('Error saving property:', err);
            setError('Failed to save property. Please try again.');
        } finally {
            setSubmitting(false);
        }
    };

    if (loading) {
        return (
            <div className="loading-container">
                <div className="loading-pulse">
                    <div className="loading-bar"></div>
                    <div className="loading-content"></div>
                    <div className="loading-line"></div>
                    <div className="loading-line"></div>
                    <div className="loading-line"></div>
                    <div className="loading-text">Loading property data...</div>
                </div>
            </div>
        );
    }

    return (
        <div className="page-container">
            <div className="container">
                <div className="form-card">
                    <div className="card-header">
                        <h1 className="card-title">
                            {isEditMode ? 'Edit Property' : 'Add New Property'}
                        </h1>
                    </div>

                    {error && (
                        <div className="error-container">
                            <div className="error-content">
                                <div className="error-icon-container">
                                    <svg className="error-icon" viewBox="0 0 20 20" fill="currentColor">
                                        <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                                    </svg>
                                </div>
                                <div className="error-message">
                                    <p>{error}</p>
                                </div>
                            </div>
                        </div>
                    )}

                    <form onSubmit={handleSubmit} className="form-container">
                        <div className="form-grid">
                            <div className="col-span-full">
                                <h2 className="section-header">Basic Information</h2>
                            </div>

                            <div className="col-span-full">
                                <label className="form-label">
                                    Title
                                </label>
                                <input
                                    type="text"
                                    name="title"
                                    value={formData.title}
                                    onChange={handleChange}
                                    required
                                    className="form-input"
                                    placeholder="Enter property title"
                                />
                            </div>

                            <div className="col-span-full">
                                <label className="form-label">
                                    Description
                                </label>
                                <textarea
                                    name="description"
                                    value={formData.description}
                                    onChange={handleChange}
                                    rows="4"
                                    className="form-textarea"
                                    placeholder="Describe the property"
                                ></textarea>
                            </div>

                            <div>
                                <label className="form-label">
                                    Price ($ per month)
                                </label>
                                <div className="price-input-container">
                                    <div className="price-symbol">
                                        <span>$</span>
                                    </div>
                                    <input
                                        type="number"
                                        name="price"
                                        value={formData.price}
                                        onChange={handleChange}
                                        min="0"
                                        step="0.01"
                                        required
                                        className="price-input"
                                        placeholder="0.00"
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="form-label">
                                    Property Type
                                </label>
                                <select
                                    name="propertyType"
                                    value={formData.propertyType}
                                    onChange={handleChange}
                                    className="form-select"
                                >
                                    <option value="Apartment">Apartment</option>
                                    <option value="House">House</option>
                                    <option value="Condo">Condo</option>
                                    <option value="Townhouse">Townhouse</option>
                                    <option value="Studio">Studio</option>
                                    <option value="Other">Other</option>
                                </select>
                            </div>

                            <div className="col-span-full section-container">
                                <h2 className="section-header">Property Details</h2>
                            </div>

                            <div>
                                <label className="form-label">
                                    Bedrooms
                                </label>
                                <input
                                    type="number"
                                    name="bedrooms"
                                    value={formData.bedrooms}
                                    onChange={handleChange}
                                    min="0"
                                    required
                                    className="form-input"
                                />
                            </div>

                            <div>
                                <label className="form-label">
                                    Bathrooms
                                </label>
                                <input
                                    type="number"
                                    name="bathrooms"
                                    value={formData.bathrooms}
                                    onChange={handleChange}
                                    min="0"
                                    step="0.5"
                                    required
                                    className="form-input"
                                />
                            </div>

                            <div>
                                <label className="form-label">
                                    Square Footage
                                </label>
                                <input
                                    type="number"
                                    name="squareFootage"
                                    value={formData.squareFootage}
                                    onChange={handleChange}
                                    min="0"
                                    required
                                    className="form-input"
                                />
                            </div>

                            <div>
                                <label className="form-label">
                                    Year Built
                                </label>
                                <input
                                    type="number"
                                    name="yearBuilt"
                                    value={formData.yearBuilt}
                                    onChange={handleChange}
                                    min="1800"
                                    max={new Date().getFullYear()}
                                    required
                                    className="form-input"
                                />
                            </div>

                            <div className="col-span-full section-container">
                                <h2 className="section-header">Property Images</h2>
                            </div>

                            <div className="col-span-full">
                                <div className="upload-container">
                                    <label className="form-label">
                                        Upload Images (Max 5MB, JPG, PNG, or GIF)
                                    </label>
                                    <div className="upload-area">
                                        <div className="upload-content">
                                            <svg className="upload-icon" stroke="currentColor" fill="none" viewBox="0 0 48 48">
                                                <path d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                                            </svg>
                                            <div className="upload-text">
                                                <label htmlFor="file-upload" className="upload-button">
                                                    <span>Upload images</span>
                                                    <input
                                                        id="file-upload"
                                                        name="file-upload"
                                                        type="file"
                                                        multiple
                                                        accept="image/jpeg,image/png,image/gif"
                                                        className="sr-only"
                                                        onChange={handleImageUpload}
                                                    />
                                                </label>
                                                <p className="upload-info">or drag and drop</p>
                                            </div>
                                            <p className="upload-hint">
                                                PNG, JPG, GIF up to 5MB
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                {imageUploadError && (
                                    <div className="upload-error">
                                        {imageUploadError}
                                    </div>
                                )}

                                {images.length > 0 && (
                                    <div className="preview-container">
                                        <h3 className="preview-title">Preview</h3>
                                        <div className="preview-grid">
                                            {images.map((image, index) => (
                                                <div key={index} className="image-card">
                                                    <img
                                                        src={image.preview}
                                                        alt={`Preview ${index + 1}`}
                                                        className="preview-image"
                                                    />
                                                    <div className="image-overlay">
                                                        <div className="delete-button-container">
                                                            <button
                                                                type="button"
                                                                onClick={() => handleRemoveImage(index)}
                                                                className="delete-button"
                                                            >
                                                                <svg className="delete-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                                                </svg>
                                                            </button>
                                                        </div>
                                                        <input
                                                            type="text"
                                                            placeholder="Add caption"
                                                            value={image.caption}
                                                            onChange={(e) => handleCaptionChange(index, e.target.value)}
                                                            className="caption-input"
                                                        />
                                                    </div>
                                                </div>
                                            ))}
                                        </div>
                                    </div>
                                )}
                            </div>

                            <div className="col-span-full section-container">
                                <h2 className="section-header">Location</h2>
                            </div>

                            <div className="col-span-full">
                                <div className="map-container">
                                    <MapLocationSelector
                                        onLocationSelect={handleLocationSelect}
                                        initialLocation={
                                            formData.latitude && formData.longitude
                                                ? { lat: parseFloat(formData.latitude), lng: parseFloat(formData.longitude) }
                                                : null
                                        }
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="form-label">
                                    Latitude
                                </label>
                                <input
                                    type="number"
                                    name="latitude"
                                    value={formData.latitude}
                                    onChange={handleChange}
                                    step="any"
                                    required
                                    className="form-input"
                                />
                            </div>

                            <div>
                                <label className="form-label">
                                    Longitude
                                </label>
                                <input
                                    type="number"
                                    name="longitude"
                                    value={formData.longitude}
                                    onChange={handleChange}
                                    step="any"
                                    required
                                    className="form-input"
                                />
                            </div>

                            <div className="col-span-full section-container">
                                <h2 className="section-header">Features</h2>
                                <div className="features-container">
                                    <div className="checkbox-container">
                                        <input
                                            type="checkbox"
                                            id="hasBalcony"
                                            name="hasBalcony"
                                            checked={formData.hasBalcony}
                                            onChange={handleChange}
                                            className="checkbox-input"
                                        />
                                        <label htmlFor="hasBalcony" className="checkbox-label">Has Balcony</label>
                                    </div>

                                    <div className="checkbox-container">
                                        <input
                                            type="checkbox"
                                            id="hasParking"
                                            name="hasParking"
                                            checked={formData.hasParking}
                                            onChange={handleChange}
                                            className="checkbox-input"
                                        />
                                        <label htmlFor="hasParking" className="checkbox-label">Has Parking</label>
                                    </div>

                                    <div className="checkbox-container">
                                        <input
                                            type="checkbox"
                                            id="petsAllowed"
                                            name="petsAllowed"
                                            checked={formData.petsAllowed}
                                            onChange={handleChange}
                                            className="checkbox-input"
                                        />
                                        <label htmlFor="petsAllowed" className="checkbox-label">Pets Allowed</label>
                                    </div>
                                </div>
                            </div>

                            <div className="button-group">
                                <button
                                    type="button"
                                    onClick={() => navigate(-1)}
                                    className="button cancel-button"
                                    disabled={submitting}
                                >
                                    Cancel
                                </button>

                                <button
                                    type="submit"
                                    disabled={submitting}
                                    className="button submit-button"
                                >
                                    {submitting ? (
                                        <span className="button-content">
                                            <svg className="loading-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                            </svg>
                                            Saving...
                                        </span>
                                    ) : (
                                        isEditMode ? 'Update Property' : 'Create Property'
                                    )}
                                </button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default PropertyFormPage;