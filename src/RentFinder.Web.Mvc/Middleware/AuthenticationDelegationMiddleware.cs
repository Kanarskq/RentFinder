namespace RentFinder.Web.Mvc.Middleware;

public class AuthenticationDelegationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationDelegationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the user is authenticated and has a token stored in session
        var token = context.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            // Add the token to the outgoing request header so it's available for HttpClient services
            context.Request.Headers.Add("Authorization", $"Bearer {token}");
        }

        await _next(context);
    }
}
