namespace RentFinder.Web.Mvc.Models;

public class BookingDetailViewModel : BookingViewModel
{
    public PropertyViewModel Property { get; set; }
    public string BookingReference { get; set; }
    public DateTime BookingDate { get; set; }
    public bool CanCancel { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
