using Microsoft.ML.Data;

namespace RealEstateSearch.Models;

public class Property
{
    [LoadColumn(0)]
    public float Price { get; set; }

    [LoadColumn(1)]
    public float SquareFootage { get; set; }

    [LoadColumn(2)]
    public float Bedrooms { get; set; }

    [LoadColumn(3)]
    public float Bathrooms { get; set; }

    [LoadColumn(4)]
    public string Location { get; set; }

    [LoadColumn(5)]
    public string PropertyType { get; set; }

    [LoadColumn(6)]
    public float YearBuilt { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime ListingDate { get; set; }

    [NoColumn]
    public List<string> Amenities { get; set; } = new List<string>();
    public string Conditions { get; set; }

    public float[] Features => new float[]
    {
            Price,
            SquareFootage,
            Bedrooms,
            Bathrooms,
            YearBuilt
    };
}
