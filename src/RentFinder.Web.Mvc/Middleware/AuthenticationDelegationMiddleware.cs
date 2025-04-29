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
        var token = context.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Add("Authorization", $"Bearer {token}");
        }

        await _next(context);
    }
}
