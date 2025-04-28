namespace Bookings.Api.Controllers.Request.Properties;

public record PropertyImageRequest(
    byte[] ImageData,
    string ImageType,
    string Caption
);
