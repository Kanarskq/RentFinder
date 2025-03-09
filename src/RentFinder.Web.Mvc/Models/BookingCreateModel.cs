namespace RentFinder.Web.Mvc.Models;

public class BookingCreateModel
{
    public int PropertyId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int GuestCount { get; set; }
    public string SpecialRequests { get; set; }
}
