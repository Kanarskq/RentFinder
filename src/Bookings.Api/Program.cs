using Bookings.Api.Apis;
using Bookings.Api.Application.Commands;
using Bookings.Api.Application.Queries;
using Bookings.Api.Infrastructure.Services;
using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Infrastructure.Repositories;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateBookingCommand).Assembly);
});

// Add HTTP context accessor for identity service
builder.Services.AddHttpContextAccessor();

// Register application services
builder.Services.AddScoped<IBookingQueries, BookingQueries>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<BookingServices>();

// Register repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Register command handlers
builder.Services.AddScoped<IRequestHandler<CreateBookingCommand, bool>, CreateBookingCommandHandler>();
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<CreateBookingCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<CreateBookingCommand, bool>));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure API endpoints
app.MapBookingApiV1();

app.Run();