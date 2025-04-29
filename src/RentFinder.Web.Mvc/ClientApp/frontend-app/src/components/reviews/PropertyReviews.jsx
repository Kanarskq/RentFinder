import React, { useState, useEffect, useContext } from 'react';
import apiClient from '../../api/apiClient';
import { reviewApi } from '../../api/reviewApi';
import { AuthContext } from '../../context/AuthContext';
import { useParams } from 'react-router-dom';
import '../../styles/ReviewStyles.css';

const PropertyReviews = () => {
    const { id } = useParams();
    const [reviews, setReviews] = useState([]);
    const [averageRating, setAverageRating] = useState(0);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showReviewForm, setShowReviewForm] = useState(false);
    const [newReview, setNewReview] = useState({ rating: 5, comment: '' });
    const { isAuthenticated, user } = useContext(AuthContext);
    const [submitLoading, setSubmitLoading] = useState(false);
    const [reviewError, setReviewError] = useState(null);

    useEffect(() => {
        const fetchReviews = async () => {
            try {
                const response = await reviewApi.getReviewsByProperty(id);
                setReviews(response.data);

                const ratingResponse = await reviewApi.getAveragePropertyRating(id);
                setAverageRating(ratingResponse.data);
            } catch (err) {
                console.error('Error fetching reviews:', err);
                if (err.response && err.response.status === 404) {
                    setReviews([]);
                    setAverageRating(0);
                } else {
                    setError('Failed to load reviews');
                }
            } finally {
                setLoading(false);
            }
        };

        if (id) {
            fetchReviews();
        }
    }, [id]);

    const handleRatingChange = (value) => {
        setNewReview({ ...newReview, rating: value });
    };

    const handleCommentChange = (e) => {
        setNewReview({ ...newReview, comment: e.target.value });
    };

    const handleSubmitReview = async (e) => {
        e.preventDefault();
        setSubmitLoading(true);
        setReviewError(null);

        try {
            await reviewApi.createReview({
                propertyId: parseInt(id),
                userId: user.id,
                rating: newReview.rating,
                comment: newReview.comment
            });

            const response = await reviewApi.getReviewsByProperty(id);
            setReviews(response.data);

            const ratingResponse = await reviewApi.getAveragePropertyRating(id);
            setAverageRating(ratingResponse.data);

            setNewReview({ rating: 5, comment: '' });
            setShowReviewForm(false);
        } catch (err) {
            console.error('Error submitting review:', err);
            setReviewError('Failed to submit review. Please try again.');
        } finally {
            setSubmitLoading(false);
        }
    };

    const renderStars = (rating) => {
        return Array(5).fill(0).map((_, index) => (
            <span key={index} className={index < rating ? 'star filled' : 'star'}>★</span>
        ));
    };

    const renderRatingInput = () => {
        return (
            <div className="rating-input">
                {[1, 2, 3, 4, 5].map((value) => (
                    <span
                        key={value}
                        className={`star ${value <= newReview.rating ? 'filled' : ''}`}
                        onClick={() => handleRatingChange(value)}
                    >
                        ★
                    </span>
                ))}
            </div>
        );
    };

    const formatDate = (dateString) => {
        try {
            if (!dateString) return 'N/A';

            const date = new Date(dateString);
            if (isNaN(date.getTime())) {
                return 'N/A';
            }
            return date.toLocaleDateString();
        } catch (err) {
            console.error('Error formatting date:', err);
            return 'N/A';
        }
    };

    if (loading) {
        return <div className="reviews-loading">Loading reviews...</div>;
    }

    return (
        <div className="property-reviews-section">
            <h2>Reviews</h2>

            <div className="reviews-summary">
                {reviews.length > 0 ? (
                    <>
                        <div className="average-rating">
                            <span className="average-score">{averageRating.toFixed(1)}</span>
                            <div className="stars">{renderStars(Math.round(averageRating))}</div>
                            <span className="review-count">({reviews.length} reviews)</span>
                        </div>
                    </>
                ) : (
                    <p>No reviews yet</p>
                )}
            </div>

            {isAuthenticated && (
                <div className="add-review-section">
                    {!showReviewForm ? (
                        <button
                            className="write-review-button"
                            onClick={() => setShowReviewForm(true)}
                        >
                            Write a Review
                        </button>
                    ) : (
                        <form className="review-form" onSubmit={handleSubmitReview}>
                            <h3>Your Review</h3>

                            <div className="rating-field">
                                <label>Rating:</label>
                                {renderRatingInput()}
                            </div>

                            <div className="comment-field">
                                <label htmlFor="reviewComment">Comment:</label>
                                <textarea
                                    id="reviewComment"
                                    rows="4"
                                    value={newReview.comment}
                                    onChange={handleCommentChange}
                                    placeholder="Share your experience with this property..."
                                    required
                                />
                            </div>

                            {reviewError && <div className="error-message">{reviewError}</div>}

                            <div className="review-form-buttons">
                                <button
                                    type="button"
                                    className="cancel-button"
                                    onClick={() => setShowReviewForm(false)}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    className="submit-button"
                                    disabled={submitLoading}
                                >
                                    {submitLoading ? 'Submitting...' : 'Submit Review'}
                                </button>
                            </div>
                        </form>
                    )}
                </div>
            )}

            <div className="reviews-list">
                {reviews.length > 0 ? (
                    reviews.map((review) => (
                        <div key={review.id} className="review-item">
                            <div className="review-header">
                                <div className="review-rating">{renderStars(review.rating)}</div>
                                <div className="review-author">by {review.userName || 'Anonymous'}</div>
                                <div className="review-date">
                                    {formatDate(review.createdAt)}
                                </div>
                            </div>
                            <div className="review-content">{review.comment}</div>
                        </div>
                    ))
                ) : (
                    <div className="no-reviews">
                        <p>Be the first to leave a review!</p>
                    </div>
                )}
            </div>
        </div>
    );
};

export default PropertyReviews;