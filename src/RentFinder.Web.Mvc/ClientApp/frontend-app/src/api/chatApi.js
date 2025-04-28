import apiClient from './apiClient';

export const chatApi = {
    getChatHistory: (userId1, userId2) => {
        return apiClient.get(`/chat/history?user1=${userId1}&user2=${userId2}`);
    },
    sendMessage: (messageData) => {
        return apiClient.post('/chat/send', messageData);
    }
};