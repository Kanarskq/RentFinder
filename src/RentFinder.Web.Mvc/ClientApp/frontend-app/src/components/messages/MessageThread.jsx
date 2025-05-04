import React, { useState, useRef, useEffect } from 'react';
import { useAuth } from '../../hooks/useAuth';
import { formatDistanceToNow } from 'date-fns';
import { parseISO, addHours } from 'date-fns';

const MessageThread = ({ conversation, messages, onSendMessage }) => {
    const [newMessage, setNewMessage] = useState('');
    const { currentUser } = useAuth();
    const messagesEndRef = useRef(null);

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (newMessage.trim()) {
            onSendMessage(newMessage);
            setNewMessage('');
        }
    };

    const formatTime = (dateString) => {
        try {
            const date = parseISO(dateString);
            const localDate = addHours(date, 2);
            return formatDistanceToNow(localDate, { addSuffix: true });
        } catch (error) {
            return 'Invalid date';
        }
    };

    return (
        <div className="message-container">
            <div className="message-header">
                <h3>
                    {conversation.otherUserName}
                    {conversation.propertyTitle && (
                        <span className="property-reference">
                            Re: {conversation.propertyTitle}
                        </span>
                    )}
                </h3>
            </div>

            <div className="message-body">
                {messages.length === 0 ? (
                    <div className="no-messages">
                        <p>Start a conversation with {conversation.otherUserName}</p>
                    </div>
                ) : (
                    <div className="messages-list">
                        {messages.map(message => (
                            <div
                                key={message.id}
                                className={`message-bubble ${message.senderId === currentUser?.id ? 'sent' : 'received'}`}
                            >
                                <div className="message-content">{message.content}</div>
                                <div className="message-meta">
                                    <span className="message-time">{formatTime(message.sentAt)}</span>
                                    {message.senderId === currentUser?.id && message.isRead && (
                                        <span className="read-status">Read</span>
                                    )}
                                </div>
                            </div>
                        ))}
                        <div ref={messagesEndRef} />
                    </div>
                )}
            </div>

            <div className="message-footer">
                <form onSubmit={handleSubmit} className="message-form">
                    <input
                        type="text"
                        value={newMessage}
                        onChange={(e) => setNewMessage(e.target.value)}
                        placeholder="Type a message..."
                        className="message-input"
                    />
                    <button type="submit" className="send-button">Send</button>
                </form>
            </div>
        </div>
    );
};

export default MessageThread;