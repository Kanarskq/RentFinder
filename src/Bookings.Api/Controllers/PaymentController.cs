using Microsoft.AspNetCore.Mvc;
using Bookings.Api.Controllers.Request.Payments;
using Bookings.Api.Infrastructure.Services.Payments;
using Microsoft.Extensions.Logging;

namespace Bookings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IConfiguration _configuration;

    public PaymentController(
        IPaymentService paymentService,
        ILogger<PaymentController> logger,
        IConfiguration configuration)
    {
        _paymentService = paymentService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("googlepay/config")]
    public IActionResult GetGooglePayConfig()
    {
        var merchantId = _configuration["Payment:GooglePay:MerchantId"];
        var gatewayMerchantId = _configuration["Stripe:GatewayMerchantId"];
        var merchantName = _configuration["Payment:MerchantName"];

        var config = new
        {
            apiVersion = 2,
            apiVersionMinor = 0,
            merchantInfo = new
            {
                merchantId,
                merchantName
            },
            allowedPaymentMethods = new[]
                {
                    new
                    {
                        type = "CARD",
                        parameters = new
                        {
                            allowedAuthMethods = new[] { "PAN_ONLY", "CRYPTOGRAM_3DS" },
                            allowedCardNetworks = new[] { "MASTERCARD", "VISA" }
                        },
                        tokenizationSpecification = new
                        {
                            type = "PAYMENT_GATEWAY",
                            parameters = new
                            {
                                gateway = "stripe",
                                gatewayMerchantId
                            }
                        }
                    }
                }
        };

        return Ok(config);
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {
        _logger.LogInformation("Processing payment for renter {RenterId} to landlord {LandlordId}",
            request.RenterId, request.LandlordId);

        try
        {
            var paymentResult = await _paymentService.ProcessPaymentAsync(request);

            if (paymentResult.Success)
            {
                _logger.LogInformation("Payment processed successfully. Transaction ID: {TransactionId}",
                    paymentResult.TransactionId);
                return Ok(paymentResult);
            }

            _logger.LogWarning("Payment failed: {ErrorMessage}", paymentResult.ErrorMessage);
            return BadRequest(paymentResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}