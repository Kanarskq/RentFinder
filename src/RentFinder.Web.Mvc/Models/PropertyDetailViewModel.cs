namespace RentFinder.Web.Mvc.Models;

public class PropertyDetailViewModel : PropertyViewModel
{
    public List<string> Amenities { get; set; }
    public string OwnerName { get; set; }
    public string OwnerContact { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<string> AdditionalImageUrls { get; set; }
    public bool IsAvailable { get; set; }
}
