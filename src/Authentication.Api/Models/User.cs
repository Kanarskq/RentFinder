namespace Authetication.Api.Models;

public class User
{
    public int UserId { get; set; }
    public string Auth0Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
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
