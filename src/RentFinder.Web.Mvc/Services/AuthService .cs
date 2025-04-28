using RentFinder.Web.Mvc.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace RentFinder.Web.Mvc.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthResponseViewModel> LoginAsync(LoginViewModel login)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(login),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseViewModel>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Store token in session
                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    _httpContextAccessor.HttpContext.Session.SetString("JwtToken", authResponse.Token);
                }

                return authResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            // Log exception
            Console.WriteLine($"Error in LoginAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthResponseViewModel> RegisterAsync(RegisterViewModel register)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(register),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseViewModel>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return authResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            // Log exception
            Console.WriteLine($"Error in RegisterAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            // Token is handled by the AuthenticationDelegationMiddleware
            var response = await _httpClient.PostAsync("api/auth/logout", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            // Log exception
            Console.WriteLine($"Error in LogoutAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<UserProfileViewModel> GetUserProfileAsync()
    {
        try
        {
            // Token is handled by the AuthenticationDelegationMiddleware
            var response = await _httpClient.GetAsync("api/auth/profile");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var userProfile = JsonSerializer.Deserialize<UserProfileViewModel>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return userProfile;
            }

            return null;
        }
        catch (Exception ex)
        {
            // Log exception
            Console.WriteLine($"Error in GetUserProfileAsync: {ex.Message}");
            return null;
        }
    }
}