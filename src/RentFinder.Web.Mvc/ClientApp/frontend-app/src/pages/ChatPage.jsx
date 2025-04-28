import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import ChatWindow from '../components/chat/ChatWindow';
import ChatList from '../components/chat/ChatList';
import { useAuth } from '../hooks/useAuth';

const ChatPage = () => {
    const { userId } = useParams();
    const { currentUser } = useAuth();
    const [conversations, setConversations] = useState([
        { userId: '1', userName: 'John Doe', unreadCount: 2 },
        { userId: '2', userName: 'Jane Smith', unreadCount: 0 },
    ]);

    if (!currentUser) {
        return (
            <div>Please log in to use chat</div>
        );
    }

    const activeConversation = conversations.find(c => c.userId === userId);

    return (
        <div className="chat-page">
            <div className="chat-container">
                <ChatList conversations={conversations} />
                {activeConversation ? (
                    <ChatWindow
                        receiverId={activeConversation.userId}
                        receiverName={activeConversation.userName}
                    />
                ) : (
                    <div className="select-conversation">
                        <p>Select a conversation to start chatting</p>
                    </div>
                )}
            </div>
        </div>
    );
};

export default ChatPage;