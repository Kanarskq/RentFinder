using Bookings.Domain.AggregatesModel.MessageAggregate;

namespace Bookings.Application.Services.Messages;

public interface IMessageService
{
    Task<IEnumerable<Conversation>> GetAllConversationsAsync(int userId);
    Task<IEnumerable<Message>> GetConversationAsync(int userId, int otherUserId);
    Task<Message> SendMessageAsync(int senderId, int receiverId, string content, int? propertyId = null);
    Task MarkConversationAsReadAsync(int userId, int otherUserId);
}
