using MediatR;

namespace Bookings.Api.Application.Commands;

public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
    where T : IRequest<R>
{
    private readonly IMediator _mediator;
    private readonly ILogger<IdentifiedCommandHandler<T, R>> _logger;
    private readonly IRequestHandler<T, R> _innerHandler;

    public IdentifiedCommandHandler(IMediator mediator, ILogger<IdentifiedCommandHandler<T, R>> logger, IRequestHandler<T, R> innerHandler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _innerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
    }

    public async Task<R> Handle(IdentifiedCommand<T, R> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing IdentifiedCommand with ID: {RequestId}", request.Id);

        var isDuplicate = await CheckForDuplicateRequestAsync(request.Id, cancellationToken);

        if (isDuplicate)
        {
            _logger.LogWarning("Duplicate request detected: {RequestId}", request.Id);
            return default!; 
        }

        var result = await _innerHandler.Handle(request.Command, cancellationToken);
        return result;
    }

    private async Task<bool> CheckForDuplicateRequestAsync(Guid requestId, CancellationToken cancellationToken)
    {
        // Заглушка
        await Task.Delay(10, cancellationToken); 
        return false; // Пока что просто возвращаем false
    }
}