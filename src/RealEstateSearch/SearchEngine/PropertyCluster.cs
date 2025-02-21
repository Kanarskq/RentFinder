using RealEstateSearch.Models;

namespace RealEstateSearch.SearchEngine;

public class PropertyCluster
{
    public string ClusterId { get; set; }
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public List<Property> Properties { get; set; }
    public double AveragePrice { get; set; }
    public int PropertyCount { get; set; }
}