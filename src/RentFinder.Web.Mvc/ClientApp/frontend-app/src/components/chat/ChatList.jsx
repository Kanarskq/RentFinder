import React from 'react';
import { Link } from 'react-router-dom';

const ChatList = ({ conversations }) => {
    return (
        <div className="chat-list">
            <h3>Your Conversations</h3>
            <ul>
                {conversations.map(conv => (
                    <li key={conv.userId}>
                        <Link to={`/chat/${conv.userId}`}>
                            {conv.userName}
                            {conv.unreadCount > 0 && (
                                <span className="unread-badge">{conv.unreadCount}</span>
                            )}
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default ChatList;