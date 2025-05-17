using Auth0.AspNetCore.Authentication;
using Bookings.Api.Controllers.Authentication;
using Bookings.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Stripe;
using Bookings.Application;
using Bookings.Application.Services.Bookings;
using Bookings.Application.Services.Messages;
using Bookings.Application.Services.Payments;
using Bookings.Application.Services.Properties;
using Bookings.Application.Services.Reviews;
using Bookings.Application.Services.Search;
using Bookings.Application.Services.Users;

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

builder.Services.AddSingleton<StripeClient>(provider =>
    new StripeClient(builder.Configuration["Stripe:SecretKey"]));

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddScoped<BookingServices>();
builder.Services.AddScoped<PropertyServices>();
builder.Services.AddScoped<ReviewServices>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

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
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

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