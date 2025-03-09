using RentFinder.Web.Mvc.Models;

namespace RentFinder.Web.Mvc.Services;

public interface IAuthService
{
    Task<AuthResponseViewModel> LoginAsync(LoginViewModel login);
    Task<AuthResponseViewModel> RegisterAsync(RegisterViewModel register);
    Task<bool> LogoutAsync();
    Task<UserProfileViewModel> GetUserProfileAsync();
}

