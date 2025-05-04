import React, { useState, useEffect } from 'react';
import GoogleMapsComponent from './GoogleMapsComponent';

const MapLocationSelector = ({ onLocationSelect, initialLocation = null }) => {
    const [showMap, setShowMap] = useState(false);
    const [center, setCenter] = useState({ lat: 50.4501, lng: 30.5234 }); // Default to Kyiv

    useEffect(() => {
        if (initialLocation &&
            initialLocation.lat !== undefined &&
            initialLocation.lng !== undefined) {
            setCenter({
                lat: typeof initialLocation.lat === 'string'
                    ? parseFloat(initialLocation.lat)
                    : initialLocation.lat,
                lng: typeof initialLocation.lng === 'string'
                    ? parseFloat(initialLocation.lng)
                    : initialLocation.lng
            });
        }
    }, [initialLocation]);

    const handleLocationSelect = (location) => {
        if (location &&
            typeof location === 'object' &&
            location.lat !== undefined &&
            location.lng !== undefined) {
            onLocationSelect(location);
        }
    };

    return (
        <div className="map-location-selector">
            <button
                type="button"
                className="select-on-map-btn"
                onClick={() => setShowMap(!showMap)}
            >
                {showMap ? 'Hide Map' : 'Select Location on Map'}
            </button>

            {showMap && (
                <div className="map-container">
                    <GoogleMapsComponent
                        apiKey={process.env.REACT_APP_GOOGLE_MAPS_API_KEY}
                        center={center}
                        zoom={12}
                        height="350px"
                        selectable={true}
                        onLocationSelect={handleLocationSelect}
                    />
                    <div className="map-instructions">
                        <p>Click on the map to select a location</p>
                    </div>
                </div>
            )}
        </div>
    );
};

export default MapLocationSelector;