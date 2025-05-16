import React, { useState, useEffect } from 'react';
import { messageApi } from '../api/messageApi';
import ConversationList from '../components/messages/ConversationList';
import MessageThread from '../components/messages/MessageThread';
import { useAuth } from '../hooks/useAuth';
import '../styles/MessageStyles.css';

const MessagesPage = () => {
    const [conversations, setConversations] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedConversation, setSelectedConversation] = useState(null);
    const [currentMessages, setCurrentMessages] = useState([]);
    const { isAuthenticated } = useAuth();

    useEffect(() => {
        if (isAuthenticated) {
            fetchConversations();
        }
    }, [isAuthenticated]);

    const fetchConversations = async () => {
        try {
            setLoading(true);
            const data = await messageApi.getAllConversations();
            setConversations(data);
            setLoading(false);
        } catch (error) {
            console.error('Failed to fetch conversations:', error);
            setLoading(false);
        }
    };

    const handleSelectConversation = async (conversation) => {
        setSelectedConversation(conversation);
        try {
            const messages = await messageApi.getConversation(conversation.otherUserId);
            setCurrentMessages(messages);

            if (!conversation.hasReadMessages) {
                await messageApi.markAsRead(conversation.id);

                setConversations(prev =>
                    prev.map(conv =>
                        conv.id === conversation.id
                            ? { ...conv, hasReadMessages: true }
                            : conv
                    )
                );
            }
        } catch (error) {
            console.error('Failed to fetch messages:', error);
        }
    };

    const handleSendMessage = async (content) => {
        if (!selectedConversation || !content.trim()) return;

        try {
            const newMessage = await messageApi.sendMessage(
                selectedConversation.otherUserId,
                content,
                selectedConversation.propertyId
            );

            setCurrentMessages(prev => [...prev, newMessage]);

            setConversations(prev =>
                prev.map(conv =>
                    conv.id === selectedConversation.id
                        ? {
                            ...conv,
                            lastMessageContent: content,
                            lastMessageSentAt: new Date().toISOString()
                        }
                        : conv
                )
            );
        } catch (error) {
            console.error('Failed to send message:', error);
        }
    };

    if (loading) {
        return <div className="loading">Loading conversations...</div>;
    }

    return (
        <div className="messages-container">
            <div className="messages-layout">
                <div className="sidebar">
                    <h2>Conversations</h2>
                    <ConversationList
                        conversations={conversations}
                        selectedId={selectedConversation?.id}
                        onSelectConversation={handleSelectConversation}
                    />
                </div>
                <div className="message-thread">
                    {selectedConversation ? (
                        <MessageThread
                            conversation={selectedConversation}
                            messages={currentMessages}
                            onSendMessage={handleSendMessage}
                        />
                    ) : (
                        <div className="empty-state">
                            <p>Select a conversation to start messaging</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default MessagesPage;