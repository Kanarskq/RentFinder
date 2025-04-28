namespace Bookings.Api.Controllers.Request.Properties;

public class AddPropertyImageRequest
{
    public IFormFile Image { get; set; }
    public string Caption { get; set; }
}
