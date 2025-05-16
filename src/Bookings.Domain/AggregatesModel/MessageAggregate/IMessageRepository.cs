using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.AggregatesModel.MessageAggregate;
using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.MessageAggregate;

public interface IMessageRepository : IRepository<Message>
{
    IUnitOfWork UnitOfWork { get; }

    Message Add(Message message);
    void Update(Message message);

    Task<IEnumerable<Message>> GetMessagesAsync(int userId, int otherUserId);
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId, int otherUserId);
    Task<Message> GetLastMessageAsync(int userId, int otherUserId);
    Task<IEnumerable<Conversation>> GetConversationsForUserAsync(int userId);
}