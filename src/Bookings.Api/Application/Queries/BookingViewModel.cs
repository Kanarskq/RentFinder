namespace Bookings.Api.Application.Queries;

public record Booking
{
    public int BookingId { get; init; }
    public int PropertyId { get; init; }
    public int UserId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public required string Status { get; init; }
}

