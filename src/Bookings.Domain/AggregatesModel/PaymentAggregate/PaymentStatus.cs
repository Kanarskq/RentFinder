namespace Bookings.Domain.AggregatesModel.PaymentAggregate;

public static class PaymentStatus
{
    public const string Pending = "Pending";
    public const string Initiated = "Initiated";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";

    public static readonly IReadOnlyList<string> AllStatuses = new[]
    {
            Pending,
            Initiated,
            Succeeded,
            Failed,
            Cancelled,
            Refunded
        };

    public static bool IsValid(string status)
    {
        return AllStatuses.Contains(status);
    }
}
