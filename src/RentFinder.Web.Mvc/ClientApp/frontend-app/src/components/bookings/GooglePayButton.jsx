import React, { useState, useEffect } from 'react';
import apiClient from '../../api/apiClient'; 

const GooglePayButton = ({
    amount,
    renterId,
    landlordId,
    description,
    onPaymentSuccess,
    onPaymentError,
    disabled
}) => {
    const [googlePayClient, setGooglePayClient] = useState(null);
    const [isGooglePayAvailable, setIsGooglePayAvailable] = useState(false);
    const [isInitializing, setIsInitializing] = useState(true);

    useEffect(() => {
        const initializeGooglePay = async () => {
            try {
                if (!window.google) {
                    await loadScript('https://pay.google.com/gp/p/js/pay.js');
                }

                const configResponse = await apiClient.get('/api/payment/googlepay/config');

                const client = new window.google.payments.api.PaymentsClient({
                    environment: 'TEST'
                });

                const { result } = await client.isReadyToPay({
                    apiVersion: 2,
                    apiVersionMinor: 0,
                    allowedPaymentMethods: configResponse.data.allowedPaymentMethods
                });

                setGooglePayClient(client);
                setIsGooglePayAvailable(result);
            } catch (error) {
                console.error('Google Pay initialization error:', error);
                onPaymentError?.('Failed to initialize Google Pay');
            } finally {
                setIsInitializing(false);
            }
        };

        initializeGooglePay();
    }, []);

    const loadScript = (src) => {
        return new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = src;
            script.onload = resolve;
            script.onerror = reject;
            document.body.appendChild(script);
        });
    };

    const handlePayment = async () => {
        if (!googlePayClient || !isGooglePayAvailable) return;

        try {
            const configResponse = await apiClient.get('/api/payment/googlepay/config');

            const paymentDataRequest = {
                ...configResponse.data,
                transactionInfo: {
                    totalPriceStatus: 'FINAL',
                    totalPrice: amount.toString(),
                    currencyCode: 'UAH',
                    countryCode: 'UA'
                },
                merchantInfo: {
                    ...configResponse.data.merchantInfo,
                    merchantId: process.env.REACT_APP_GOOGLE_PAY_TEST_MERCHANT 
                }
            };

            const paymentData = await googlePayClient.loadPaymentData(paymentDataRequest);

            const paymentResult = await processPayment(
                paymentData.paymentMethodData.tokenizationData.token
            );

            if (paymentResult.success) {
                onPaymentSuccess?.(paymentResult);
            } else {
                onPaymentError?.(paymentResult.errorMessage || 'Payment failed');
            }
        } catch (error) {
            if (error.statusCode === 'CANCELED') {
                console.log('Payment was canceled by user');
            } else {
                console.error('Google Pay error:', error);
                onPaymentError?.('Payment processing failed');
            }
        }
    };

    const processPayment = async (paymentToken) => {
        try {
            const response = await apiClient.post('/api/payment/process', {
                paymentToken,
                renterId,
                landlordId,
                amount,
                currency: 'UAH',
                description
            });
            return response.data;
        } catch (error) {
            console.error('Payment processing error:', error);
            throw error;
        }
    };

    if (isInitializing) {
        return <div className="gp-loading">Initializing Google Pay...</div>;
    }

    if (!isGooglePayAvailable) {
        return null;
    }

    return (
        <button
            className="google-pay-button"
            onClick={handlePayment}
            disabled={disabled || !isGooglePayAvailable}
            aria-label="Pay with Google Pay"
        >
            <div className="google-pay-logo" />
            <span>Pay with Google Pay</span>
        </button>
    );
};

export default GooglePayButton;