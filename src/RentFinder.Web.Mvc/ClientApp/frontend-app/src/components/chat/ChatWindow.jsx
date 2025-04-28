import React, { useState, useRef, useEffect } from 'react';
import { useChat } from '../../hooks/useChat';

const ChatWindow = ({ receiverId, receiverName }) => {
    const [message, setMessage] = useState('');
    const { messages, sendMessage, loading, error } = useChat();
    const messagesEndRef = useRef(null);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (message.trim()) {
            sendMessage(receiverId, message);
            setMessage('');
        }
    };

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const filteredMessages = messages.filter(
        msg => (msg.senderId === receiverId || msg.receiverId === receiverId)
    );

    return (
        <div className="chat-window">
            <div className="chat-header">
                <h3>Chat with {receiverName}</h3>
            </div>

            {loading && <div>Loading chat...</div>}
            {error && <div className="error">{error}</div>}

            <div className="messages-container">
                {filteredMessages.map((msg, index) => (
                    <div
                        key={index}
                        className={`message ${msg.isOwn ? 'own-message' : 'other-message'}`}
                    >
                        <div className="message-content">
                            {msg.message}
                        </div>
                    </div>
                ))}
                <div ref={messagesEndRef} />
            </div>

            <form onSubmit={handleSubmit} className="message-form">
                <input
                    type="text"
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder="Type your message..."
                />
                <button type="submit">Send</button>
            </form>
        </div>
    );
};

export default ChatWindow;