namespace Bookings.Domain.AggregatesModel.PaymentAggregate;

public static class PaymentMethod
{
    public const string GooglePay = "GooglePay";

    public static readonly IReadOnlyList<string> AllMethods = new[]
    {
        GooglePay,
    };

    public static bool IsValid(string method)
    {
        return AllMethods.Contains(method);
    }
}
