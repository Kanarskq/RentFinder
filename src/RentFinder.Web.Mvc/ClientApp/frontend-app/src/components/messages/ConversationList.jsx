import React from 'react';
import { formatDistanceToNow, parseISO, addHours } from 'date-fns';

const ConversationList = ({ conversations, selectedId, onSelectConversation }) => {
    if (!conversations.length) {
        return <div className="empty-conversations">No conversations yet</div>;
    }

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
        <div className="conversation-list">
            {conversations.map(conversation => (
                <div
                    key={conversation.id}
                    className={`conversation-item ${selectedId === conversation.id ? 'selected' : ''} ${!conversation.hasReadMessages ? 'unread' : ''}`}
                    onClick={() => onSelectConversation(conversation)}
                >
                    <div className="conversation-user">
                        <h4>{conversation.otherUserName}</h4>
                        {conversation.propertyTitle && (
                            <span className="property-title">
                                Re: {conversation.propertyTitle}
                            </span>
                        )}
                    </div>
                    <div className="conversation-preview">
                        <p>{conversation.lastMessageContent}</p>
                        <span className="message-time">{formatTime(conversation.lastMessageSentAt)}</span>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default ConversationList;