var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();


var app = builder.Build();

app.UseHttpsRedirection();


app.Run();