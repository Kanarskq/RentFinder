import React from 'react';
import { Link } from 'react-router-dom';

const Footer = () => {
    return (
        <footer className="app-footer">
            <div className="footer-container">
                <div className="footer-section">
                    <h4>RentFinder</h4>
                    <p>Find your perfect rental property with ease.</p>
                </div>
            </div>

            <div className="footer-bottom">
                <p>&copy; {new Date().getFullYear()} RentFinder. All rights reserved.</p>
            </div>
        </footer>
    );
};

export default Footer;