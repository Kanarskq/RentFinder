import apiClient from './apiClient';

export const messageApi = {
    getAllConversations: async () => {
        try {
            const response = await apiClient.get('/api/messages/conversations');
            return response.data;
        } catch (error) {
            console.error('Error fetching conversations:', error);
            throw error;
        }
    },

    getConversation: async (otherUserId) => {
        try {
            const response = await apiClient.get(`/api/messages/conversation/${otherUserId}`);
            return response.data;
        } catch (error) {
            console.error('Error fetching conversation:', error);
            throw error;
        }
    },

    sendMessage: async (receiverId, content, propertyId) => {
        try {
            const response = await apiClient.post('/api/messages/send', {
                receiverId,
                content,
                propertyId
            });
            return response.data;
        } catch (error) {
            console.error('Error sending message:', error);
            throw error;
        }
    },

    markAsRead: async (conversationId) => {
        try {
            const response = await apiClient.put(`/api/messages/read/${conversationId}`);
            return response.data;
        } catch (error) {
            console.error('Error marking conversation as read:', error);
            throw error;
        }
    }
};