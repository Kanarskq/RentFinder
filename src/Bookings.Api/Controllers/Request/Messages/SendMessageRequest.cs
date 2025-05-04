namespace Bookings.Api.Controllers.Request.Messages;

public record SendMessageRequest(
    int ReceiverId,
    string Content,
    int? PropertyId
);
