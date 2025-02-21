using Bookings.Api.Application.Commands;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Api.Apis;

public static class BookingApi
{
    public static RouteGroupBuilder MapBookingApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/booking");

        api.MapPost("/", CreateBookingAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>>> CreateBookingAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        CreateBookingRequest request,
        [AsParameters] BookingServices services)
    {
        services.Logger.LogInformation(
            "Received booking request for UserId: {UserId} using GooglePay",
            request.UserId);

        if (requestId == Guid.Empty)
        {
            services.Logger.LogWarning("Invalid Request - RequestId is missing");
            return TypedResults.BadRequest("RequestId is missing.");
        }

        using (services.Logger.BeginScope(new List<KeyValuePair<string, object>> { new("IdentifiedCommandId", requestId) }))
        {
            var createBookingCommand = new CreateBookingCommand(
                request.PropertyId,
                request.UserId,
                request.StartDate,
                request.EndDate
            );

            var requestCreateBooking = new IdentifiedCommand<CreateBookingCommand, bool>(createBookingCommand, requestId);

            services.Logger.LogInformation("Sending CreateBookingCommand for UserId: {UserId} using GooglePay", request.UserId);

            var result = await services.Mediator.Send(requestCreateBooking);

            if (result)
            {
                services.Logger.LogInformation("CreateBookingCommand succeeded - RequestId: {RequestId}", requestId);
                return TypedResults.Ok();
            }
            else
            {
                services.Logger.LogWarning("CreateBookingCommand failed - RequestId: {RequestId}", requestId);
                return TypedResults.BadRequest("Failed to create booking.");
            }
        }
    }
}

public record CreateBookingRequest(
    int BookingId,
    int PropertyId,
    int UserId,
    DateTime StartDate,
    DateTime EndDate,
    DateTime CreatedAt,
    string Status
);
