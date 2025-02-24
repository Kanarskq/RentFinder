using RealEstateSearch.Models;

namespace RealEstateSearch.SearchEngine;

public class SearchCriteria
{
    public float? MinPrice { get; set; }
    public float? MaxPrice { get; set; }
    public float? MinSquareFootage { get; set; }
    public float? MaxSquareFootage { get; set; }
    public float? MinBedrooms { get; set; }
    public float? MaxBedrooms { get; set; }
    public string Location { get; set; }
    public string PropertyType { get; set; }
    public int? MaxAge { get; set; }
    public double? MaxDistance { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public List<string> RequiredAmenities { get; set; }
    public string Condition { get; set; }
    public int? DaysOnMarket { get; set; }
}
