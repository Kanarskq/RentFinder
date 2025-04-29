using RentFinder.Web.Mvc.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace RentFinder.Web.Mvc.Services;

public class BookingService : IBookingService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookingService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BookingResponseViewModel> CreateBookingAsync(BookingCreateModel bookingModel)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(bookingModel),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("api/bookings", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var bookingResponse = JsonSerializer.Deserialize<BookingResponseViewModel>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return bookingResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateBookingAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<BookingViewModel>> GetUserBookingsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/bookings/user");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var bookings = JsonSerializer.Deserialize<IEnumerable<BookingViewModel>>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return bookings;
            }

            return Array.Empty<BookingViewModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserBookingsAsync: {ex.Message}");
            return Array.Empty<BookingViewModel>();
        }
    }

    public async Task<BookingDetailViewModel> GetBookingDetailAsync(int bookingId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/bookings/{bookingId}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var bookingDetail = JsonSerializer.Deserialize<BookingDetailViewModel>(
                    responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return bookingDetail;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetBookingDetailAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CancelBookingAsync(int bookingId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/bookings/{bookingId}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CancelBookingAsync: {ex.Message}");
            return false;
        }
    }
}
