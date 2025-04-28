using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.Services;

public class GooglePayOptions
{
    public const string GooglePay = "GooglePay";

    public string ApiKey { get; set; } = "";
    public string MerchantId { get; set; } = "";
    public string MerchantName { get; set; } = "";
    public string ApiUrl { get; set; } = "";
    public string Environment { get; set; } = "TEST"; // TEST or PRODUCTION
    public bool AllowedCardNetworks { get; set; } = true;
    public string[] SupportedNetworks { get; set; } = new[] { "VISA", "MASTERCARD" };
}
