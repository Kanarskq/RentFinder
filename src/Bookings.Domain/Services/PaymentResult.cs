using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Domain.Services;
public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = "";
    public string ProviderReference { get; set; } = "";
    public string PaymentIntent { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
    public string Status { get; set; } = "";
}
