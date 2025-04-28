using Auth0.AspNetCore.Authentication;
using Bookings.Api.Application.Commands.Bookings;
using Bookings.Api.Application.Commands.Identities;
using Bookings.Api.Application.Commands.Properties;
using Bookings.Api.Application.Commands.Reviews;
using Bookings.Api.Application.Queries.Bookings;
using Bookings.Api.Application.Queries.Properties;
using Bookings.Api.Application.Queries.Reviews;
using Bookings.Api.Controllers.Authentication;
using Bookings.Api.Infrastructure.Services;
using Bookings.Api.Infrastructure.Services.Bookings;
using Bookings.Api.Infrastructure.Services.Properties;
using Bookings.Api.Infrastructure.Services.Reviews;
using Bookings.Api.Infrastructure.Services.Search;
using Bookings.Api.Infrastructure.Services.Users;
using Bookings.Domain.AggregatesModel.BookingAggregate;
using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Bookings.Domain.AggregatesModel.ReviewAggregate;
using Bookings.Infrastructure;
using Bookings.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString().Replace("+", "_"));
    options.CustomOperationIds(apiDescription =>
        apiDescription.TryGetMethodInfo(out var methodInfo)
            ? methodInfo.Name
            : null);
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
    var key = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {key, Array.Empty<string>()}
    };
    options.AddSecurityRequirement(securityRequirement);
    options.UseAllOfToExtendReferenceSchemas();
});

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateBookingCommand).Assembly);
});

builder.Services.AddScoped<IBookingQueries, BookingQueries>();
builder.Services.AddScoped<IPropertyQueries, PropertyQueries>();
builder.Services.AddScoped<IReviewQueries, ReviewQueries>();
builder.Services.AddScoped<BookingServices>();
builder.Services.AddScoped<PropertyServices>();
builder.Services.AddScoped<ReviewServices>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();


builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IRequestHandler<CreateBookingCommand, bool>, CreateBookingCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CancelBookingCommand, bool>, CancelBookingCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CreatePropertyCommand, bool>, CreatePropertyCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdatePropertyCommand, bool>, UpdatePropertyCommandHandler>();
builder.Services.AddScoped<IRequestHandler<MarkPropertyAsAvailableCommand, bool>, MarkPropertyAsAvailableCommandHandler>();
builder.Services.AddScoped<IRequestHandler<MarkPropertyAsUnavailableCommand, bool>, MarkPropertyAsUnavailableCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CreateReviewCommand, bool>, CreateReviewCommandHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateReviewCommand, bool>, UpdateReviewCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AddPropertyImageCommand, bool>, AddPropertyImageCommandHandler>();
builder.Services.AddScoped<IRequestHandler<DeletePropertyImageCommand, bool>, DeletePropertyImageCommandHandler>();

builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<CreateBookingCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<CreateBookingCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<CancelBookingCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<CancelBookingCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<CreatePropertyCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<CreatePropertyCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<UpdatePropertyCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<UpdatePropertyCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<MarkPropertyAsAvailableCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<MarkPropertyAsAvailableCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<MarkPropertyAsUnavailableCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<MarkPropertyAsUnavailableCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<CreateReviewCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<CreateReviewCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<UpdateReviewCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<UpdateReviewCommand, bool>)); ;
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<AddPropertyImageCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<AddPropertyImageCommand, bool>));
builder.Services.AddScoped(typeof(IRequestHandler<IdentifiedCommand<DeletePropertyImageCommand, bool>, bool>),
    typeof(IdentifiedCommandHandler<DeletePropertyImageCommand, bool>));

builder.Services.AddScoped<PropertySearchEngine>();
builder.Services.AddScoped<IPropertySearchEngine, PropertySearchEngine>();

builder.Services.AddAuth0WebAppAuthentication(
        options =>
        {
            options.Domain = builder.Configuration["Auth0:Domain"];
            options.ClientId = builder.Configuration["Auth0:ClientId"];
            options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        })
    .WithAccessToken(options =>
    {
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.Scope = "openid profile email offline_access audience permissions issuer read:messages";
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Auth0Constants.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    options.AddPolicy("RequireTenantRole", policy => policy.RequireRole("tenant"));
    options.AddPolicy("RequireLessorRole", policy => policy.RequireRole("lessor"));
    options.InvokeHandlersAfterFailure = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var allowedOrigins = builder.Configuration["AllowedOrigins:ReactApp"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAppPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddRentFinderPersistence(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ReactAppPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();