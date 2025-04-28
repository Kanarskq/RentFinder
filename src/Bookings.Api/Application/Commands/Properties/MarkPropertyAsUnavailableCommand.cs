using MediatR;

namespace Bookings.Api.Application.Commands.Properties;

public record MarkPropertyAsUnavailableCommand(int PropertyId) : IRequest<bool>;