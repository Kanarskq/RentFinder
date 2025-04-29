using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Bookings.Domain.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Bookings.Infrastructure.Services;

public class GooglePayService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly GooglePayOptions _options;

    //public string PaymentMethod => PaymentMethod.GooglePay;

    public GooglePayService(HttpClient httpClient, IOptions<GooglePayOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        _httpClient.BaseAddress = new Uri(_options.ApiUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public string PaymentMethod => throw new NotImplementedException();

    public async Task<PaymentResult> InitiatePaymentAsync(PaymentRequest request)
    {
        try
        {
            var requestData = new
            {
                amount = request.Amount,
                currency = request.Currency,
                description = request.Description,
                merchantId = _options.MerchantId,
                metadata = new
                {
                    bookingId = request.BookingId,
                    userId = request.UserId
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/payments/create", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new PaymentResult
                {
                    Success = false,
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Failed to initiate payment: {errorContent}"
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePayInitiateResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new PaymentResult
            {
                Success = true,
                PaymentIntent = result?.PaymentIntent ?? "",
                Status = PaymentStatus.Initiated
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult
            {
                Success = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = $"Exception while initiating payment: {ex.Message}"
            };
        }
    }

    public async Task<PaymentResult> ProcessPaymentAsync(string paymentIntent, string paymentToken)
    {
        try
        {
            var requestData = new
            {
                paymentIntent,
                paymentToken,
                merchantId = _options.MerchantId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/payments/process", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new PaymentResult
                {
                    Success = false,
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Failed to process payment: {errorContent}"
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePayProcessResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new PaymentResult
            {
                Success = true,
                TransactionId = result?.TransactionId ?? "",
                ProviderReference = result?.ProviderReference ?? "",
                Status = PaymentStatus.Succeeded
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult
            {
                Success = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = $"Exception while processing payment: {ex.Message}"
            };
        }
    }

    public async Task<PaymentResult> RefundPaymentAsync(string transactionId, decimal amount)
    {
        try
        {
            var requestData = new
            {
                transactionId,
                amount,
                merchantId = _options.MerchantId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/payments/refund", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new PaymentResult
                {
                    Success = false,
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Failed to refund payment: {errorContent}"
                };
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GooglePayRefundResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                ProviderReference = result?.RefundReference ?? "",
                Status = PaymentStatus.Refunded
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult
            {
                Success = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = $"Exception while refunding payment: {ex.Message}"
            };
        }
    }

    private string MapGooglePayStatus(string googlePayStatus)
    {
        return googlePayStatus switch
        {
            "PENDING" => PaymentStatus.Pending,
            "PROCESSING" => PaymentStatus.Initiated,
            "COMPLETED" => PaymentStatus.Succeeded,
            "FAILED" => PaymentStatus.Failed,
            "CANCELLED" => PaymentStatus.Cancelled,
            "REFUNDED" => PaymentStatus.Refunded,
            _ => googlePayStatus
        };
    }

    private class GooglePayInitiateResponse
    {
        public string PaymentIntent { get; set; } = "";
        public string ClientSecret { get; set; } = "";
    }

    private class GooglePayProcessResponse
    {
        public string TransactionId { get; set; } = "";
        public string ProviderReference { get; set; } = "";
        public string Status { get; set; } = "";
    }

    private class GooglePayRefundResponse
    {
        public string RefundReference { get; set; } = "";
        public string Status { get; set; } = "";
    }

    private class GooglePayStatusResponse
    {
        public string Status { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    }
}
