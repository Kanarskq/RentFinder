using Bookings.Domain.AggregatesModel.MessageAggregate;
using Bookings.Domain.SeedWork;
using Bookings.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly BookingContext _context;

    public MessageRepository(BookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public Message Add(Message message)
    {
        return _context.Messages.Add(message).Entity;
    }

    public void Update(Message message)
    {
        _context.Messages.Update(message);
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(int userId, int otherUserId)
    {
        return await _context.Messages
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId, int otherUserId)
    {
        return await _context.Messages
            .Where(m =>
                m.ReceiverId == userId &&
                m.SenderId == otherUserId &&
                m.ReadAt == null)
            .ToListAsync();
    }

    public async Task<Message> GetLastMessageAsync(int userId, int otherUserId)
    {
        return await _context.Messages
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Conversation>> GetConversationsForUserAsync(int userId)
    {
        var conversationPartners = await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var conversations = new List<Conversation>();

        foreach (var partnerId in conversationPartners)
        {
            conversations.Add(new Conversation
            {
                Id = $"conversation-{partnerId}",
                OtherUserId = partnerId
            });
        }

        return conversations;
    }
}