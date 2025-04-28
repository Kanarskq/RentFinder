using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Domain.Services;

public class PaymentRequest
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Description { get; set; } = "";
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}
