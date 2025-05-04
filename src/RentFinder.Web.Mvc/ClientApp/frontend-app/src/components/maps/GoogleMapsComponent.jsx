import React, { useRef, useEffect, useState } from 'react';
import { Loader } from '@googlemaps/js-api-loader';

const GoogleMapsComponent = ({
    apiKey,
    center = { lat: 50.4501, lng: 30.5234 }, // Default to Kyiv
    zoom = 14,
    markers = [],
    height = '400px',
    selectable = false,
    onLocationSelect = () => { }
}) => {
    const mapRef = useRef(null);
    const [map, setMap] = useState(null);
    const [selectedLocation, setSelectedLocation] = useState(null);
    const [mapLoaded, setMapLoaded] = useState(false);
    const [error, setError] = useState(null);
    const markersRef = useRef([]);

    const googleMapsApiKey = apiKey || process.env.REACT_APP_GOOGLE_MAPS_API_KEY;

    const clearMarkers = () => {
        if (markersRef.current && markersRef.current.length > 0) {
            markersRef.current.forEach(marker => {
                if (marker) marker.setMap(null);
            });
            markersRef.current = [];
        }
    };

    useEffect(() => {
        if (!mapRef.current) return;

        const initializeMap = async () => {
            try {
                if (!googleMapsApiKey) {
                    setError("Google Maps API key is missing");
                    console.error("Google Maps API key is missing");
                    return;
                }

                const loader = new Loader({
                    apiKey: googleMapsApiKey,
                    version: "weekly",
                    libraries: ["places"]
                });

                await loader.load();
                setMapLoaded(true);

                const numericCenter = {
                    lat: typeof center.lat === 'string' ? parseFloat(center.lat) : center.lat,
                    lng: typeof center.lng === 'string' ? parseFloat(center.lng) : center.lng
                };

                const googleMap = new window.google.maps.Map(mapRef.current, {
                    center: numericCenter,
                    zoom,
                    mapTypeControl: true,
                    streetViewControl: true,
                    fullscreenControl: true
                });

                setMap(googleMap);

                if (selectable) {
                    const marker = new window.google.maps.Marker({
                        map: googleMap,
                        draggable: true,
                        visible: false
                    });

                    googleMap.addListener('click', (event) => {
                        const location = {
                            lat: event.latLng.lat(),
                            lng: event.latLng.lng()
                        };

                        marker.setPosition(location);
                        marker.setVisible(true);

                        setSelectedLocation(location);
                        onLocationSelect(location);
                    });
                }
            } catch (err) {
                console.error("Error initializing Google Maps:", err);
                setError(`Failed to load Google Maps: ${err.message}`);
            }
        };

        initializeMap();

        return () => {
            clearMarkers();
        };
    }, [googleMapsApiKey]); 

    useEffect(() => {
        if (map && markers && markers.length > 0) {
            clearMarkers();

            const newMarkers = markers.map(markerData => {
                if (!markerData || !markerData.lat || !markerData.lng) {
                    return null;
                }

                const marker = new window.google.maps.Marker({
                    position: {
                        lat: Number(markerData.lat),
                        lng: Number(markerData.lng)
                    },
                    map: map,
                    title: markerData.title || ''
                });

                if (markerData.info) {
                    const infoWindow = new window.google.maps.InfoWindow({
                        content: markerData.info
                    });

                    marker.addListener('click', () => {
                        infoWindow.open(map, marker);
                    });
                }

                return marker;
            }).filter(marker => marker !== null);

            markersRef.current = newMarkers;
        }
    }, [map, markers]);

    useEffect(() => {
        if (map && center && center.lat && center.lng) {
            const numericCenter = {
                lat: typeof center.lat === 'string' ? parseFloat(center.lat) : center.lat,
                lng: typeof center.lng === 'string' ? parseFloat(center.lng) : center.lng
            };

            map.setCenter(numericCenter);
        }
    }, [map, center]);

    return (
        <div className="google-maps-component">
            {error && (
                <div className="map-error-message" style={{ color: 'red', marginBottom: '10px' }}>
                    {error}
                </div>
            )}
            <div
                ref={mapRef}
                style={{
                    width: '100%',
                    height,
                    borderRadius: '8px',
                    backgroundColor: '#f8f8f8'
                }}
            ></div>
            {selectable && selectedLocation && (
                <div className="selected-coordinates">
                    <p>Selected coordinates: {selectedLocation.lat.toFixed(6)}, {selectedLocation.lng.toFixed(6)}</p>
                </div>
            )}
        </div>
    );
};

export default GoogleMapsComponent;