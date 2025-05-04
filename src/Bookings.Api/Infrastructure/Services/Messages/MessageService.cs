using Bookings.Api.Application.Queries.Properties;
using Bookings.Api.Application.Queries.Users;
using Bookings.Api.Infrastructure.Messages;
using Bookings.Api.Infrastructure.Services.Properties;
using Bookings.Api.Infrastructure.Services.Users;
using Bookings.Domain.AggregatesModel.MessageAggregate;
using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Api.Infrastructure.Services.Messages;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserService _userService;
    private readonly IUserQueries _userQueries;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(
        IMessageRepository messageRepository,
        IUserService userService,
        IUserQueries userQueries)
    {
        _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
        _unitOfWork = messageRepository.UnitOfWork;
    }

    public async Task<IEnumerable<Conversation>> GetAllConversationsAsync(int userId)
    {
        var conversations = await _messageRepository.GetConversationsForUserAsync(userId);

        var enrichedConversations = new List<Conversation>();

        foreach (var conversation in conversations)
        {
            var otherUserId = conversation.OtherUserId;
            var otherUserName = await _userQueries.GetUserNameById(otherUserId);

            conversation.OtherUserName = otherUserName;

            var lastMessage = await _messageRepository.GetLastMessageAsync(userId, otherUserId);
            if (lastMessage != null)
            {
                conversation.LastMessageContent = lastMessage.Content;
                conversation.LastMessageSentAt = lastMessage.SentAt;
                conversation.HasReadMessages = lastMessage.SenderId == userId || lastMessage.IsRead;
                conversation.PropertyId = lastMessage.PropertyId;
            }

            enrichedConversations.Add(conversation);
        }

        return enrichedConversations.OrderByDescending(c => c.LastMessageSentAt);
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(int userId, int otherUserId)
    {
        var messages = await _messageRepository.GetMessagesAsync(userId, otherUserId);
        return messages.OrderBy(m => m.SentAt);
    }

    public async Task<Message> SendMessageAsync(int senderId, int receiverId, string content, int? propertyId = null)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow,
            PropertyId = propertyId
        };

        _messageRepository.Add(message);
        await _unitOfWork.SaveEntitiesAsync();

        return message;
    }

    public async Task MarkConversationAsReadAsync(int userId, int otherUserId)
    {
        var unreadMessages = await _messageRepository.GetUnreadMessagesAsync(userId, otherUserId);

        foreach (var message in unreadMessages)
        {
            message.ReadAt = DateTime.UtcNow;
            _messageRepository.Update(message);
        }

        await _unitOfWork.SaveEntitiesAsync();
    }
}