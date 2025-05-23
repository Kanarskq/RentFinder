﻿using Bookings.Application.Queries.Properties;
using Bookings.Application.Services.Search;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookings.Application.Services.Properties;


public class PropertyServices
{
    public ILogger<PropertyServices> Logger { get; }
    public IMediator Mediator { get; }
    public IPropertyQueries Queries { get; }
    public PropertySearchEngine SearchEngine { get; }

    public PropertyServices(
        ILogger<PropertyServices> logger,
        IMediator mediator,
        IPropertyQueries queries,
        PropertySearchEngine searchEngine)
    {
        Queries = queries ?? throw new ArgumentNullException(nameof(queries));
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        SearchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
    }
}