namespace RentFinder.Web.Mvc.Models;

public class UserProfileViewModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime RegisteredDate { get; set; }
    public List<BookingViewModel> RecentBookings { get; set; }
}