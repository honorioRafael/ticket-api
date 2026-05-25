using Sales.API.Jobs;
using Sales.Application;
using Sales.Infrastructure;
using SharedKernel.Middlewares;

SharedKernel.EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddSalesApplication();
builder.Services.AddSalesInfrastructure(connectionString);

builder.Services.AddHostedService<ReservationExpiryJob>();
builder.Services.AddHostedService<EventFinisherJob>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
