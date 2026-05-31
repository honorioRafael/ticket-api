using Sales.API.Jobs;
using Sales.Application;
using Sales.Infrastructure;
using TicketApi.Common.Middlewares;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
TicketApi.Common.EnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A string de conexão 'DefaultConnection' não foi encontrada.");

builder.Services.AddSalesApplication();
builder.Services.AddSalesInfrastructure(connectionString);

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
