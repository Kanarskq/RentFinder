import React, { useState } from 'react';
import GooglePayButton from './GooglePayButton';

const PaymentPage = ({ propertyId, rentalInfo }) => {
    const [paymentStatus, setPaymentStatus] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handlePaymentSuccess = (result) => {
        setPaymentStatus(`Оплата успешно проведена! ID транзакции: ${result.transactionId}`);
    };

    const handlePaymentError = (errorMessage) => {
        setPaymentStatus(`Ошибка: ${errorMessage}`);
    };

    return (
        <div className="payment-page">
            <h2>Оплата аренды</h2>
            <div className="property-details">
                <h3>{rentalInfo.propertyName}</h3>
                <p>Адрес: {rentalInfo.address}</p>
                <p>Арендодатель: {rentalInfo.landlordName}</p>
                <p>Период: {rentalInfo.rentalPeriod}</p>
                <h4>Сумма к оплате: {rentalInfo.amount} грн</h4>
            </div>

            <div className="payment-methods">
                <GooglePayButton
                    amount={rentalInfo.amount}
                    renterId={rentalInfo.renterId}
                    landlordId={rentalInfo.landlordId}
                    description={`Оплата аренды: ${rentalInfo.propertyName} за ${rentalInfo.rentalPeriod}`}
                    onPaymentSuccess={handlePaymentSuccess}
                    onPaymentError={handlePaymentError}
                />
            </div>

            {paymentStatus && (
                <div className={`payment-status ${paymentStatus.includes('Ошибка') ? 'error' : 'success'}`}>
                    {paymentStatus}
                </div>
            )}
        </div>
    );
};

export default PaymentPage;