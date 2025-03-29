namespace RentFinder.Web.Mvc.Models;

public class PropertySearchModel
{
    public string Location { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public double? MinArea { get; set; }
    public string PropertyType { get; set; }
}
