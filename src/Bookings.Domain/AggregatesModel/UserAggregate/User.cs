using Bookings.Domain.SeedWork;

namespace Bookings.Domain.AggregatesModel.UserAggregate;

public class User : Entity, IAggregateRoot
{
    public int Id { get; set; }
    public string Auth0Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Name { get; set; }
    public string Role { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public DateTime? LastLogin { get; set; }
    public string? PhoneNumber { get; set; }
    public bool EmailVerified { get; set; }
}

public static class UserRoles
{
    public const string Admin = "admin";
    public const string Tenant = "tenant";
    public const string Lessor = "lessor";

    public static readonly IReadOnlyList<string> AllRoles = new[] { Admin, Tenant, Lessor };
}

