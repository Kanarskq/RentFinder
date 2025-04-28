using System.ComponentModel.DataAnnotations;

namespace Bookings.Api.Controllers.Request.Properties;

public class SimilarPropertySearchRequest
{
    public decimal Price { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public float SquareFootage { get; set; }
    public int Bedrooms { get; set; }
    public float Bathrooms { get; set; }
    public int YearBuilt { get; set; }
    public bool HasBalcony { get; set; }
    public bool HasParking { get; set; }
    public bool PetsAllowed { get; set; }
    public string PropertyType { get; set; }
    public int? MaxResults { get; set; }
}
