import { useState, useEffect, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuth } from './useAuth';

export const useChat = () => {
    const { currentUser } = useAuth();
    const [connection, setConnection] = useState(null);
    const [messages, setMessages] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const startConnection = useCallback(async () => {
        try {
            setLoading(true);
            const newConnection = new signalR.HubConnectionBuilder()
                .withUrl('/chatHub')
                .withAutomaticReconnect()
                .build();

            await newConnection.start();
            await newConnection.invoke('JoinChat', currentUser.id);

            newConnection.on('ReceiveMessage', (senderId, message) => {
                setMessages(prev => [...prev, { senderId, message, isOwn: false }]);
            });

            setConnection(newConnection);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, [currentUser]);

    const sendMessage = useCallback(async (receiverId, message) => {
        if (!connection) return;

        try {
            await connection.invoke('SendMessage', currentUser.id, receiverId, message);
            setMessages(prev => [...prev, { receiverId, message, isOwn: true }]);
        } catch (err) {
            setError(err.message);
        }
    }, [connection, currentUser]);

    useEffect(() => {
        if (currentUser) {
            startConnection();
        }

        return () => {
            if (connection) {
                connection.stop();
            }
        };
    }, [currentUser, startConnection]);

    return { messages, sendMessage, loading, error };
};