namespace Bookings.Api.Infrastructure.Services;

public interface IIdentityService
{
    string GetUserIdentity();

    string GetUserName();
}
