namespace RentFinder.Web.Mvc.Models;

public class AuthResponseViewModel
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime Expiration { get; set; }
}