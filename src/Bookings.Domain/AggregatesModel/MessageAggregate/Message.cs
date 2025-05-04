using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.MessageAggregate;

public class Message : Entity, IAggregateRoot
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsRead => ReadAt.HasValue;
    public int? PropertyId { get; set; } 
}
