namespace Bookings.Domain.AggregatesModel.MessageAggregate;

public class Conversation
{
    public string Id { get; set; }
    public int OtherUserId { get; set; }
    public string OtherUserName { get; set; }
    public string LastMessageContent { get; set; }
    public DateTime LastMessageSentAt { get; set; }
    public bool HasReadMessages { get; set; }
    public int? PropertyId { get; set; } 
    public string PropertyTitle { get; set; } 
}
