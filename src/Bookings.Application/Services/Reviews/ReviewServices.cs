using Bookings.Application.Queries.Reviews;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookings.Application.Services.Reviews;

public class ReviewServices
{
    public ILogger<ReviewServices> Logger { get; }
    public IMediator Mediator { get; }
    public IReviewQueries Queries { get; }

    public ReviewServices(
        ILogger<ReviewServices> logger,
        IMediator mediator,
        IReviewQueries queries)
    {
        Logger = logger;
        Mediator = mediator;
        Queries = queries;
    }
}