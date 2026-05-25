using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sales.Domain.Repositories;

namespace Sales.API.Jobs;

public class EventFinisherJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventFinisherJob> _logger;

    public EventFinisherJob(IServiceProvider serviceProvider, ILogger<EventFinisherJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Job de finalização de eventos iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var now = DateTime.UtcNow;
                    var endedEvents = await eventRepository.GetPublishedEventsEndingBeforeAsync(now, stoppingToken);

                    if (endedEvents.Count > 0)
                    {
                        _logger.LogInformation("Encontrados {Count} eventos encerrados para finalizar.", endedEvents.Count);

                        foreach (var @event in endedEvents)
                        {
                            @event.Finish();
                            eventRepository.Update(@event);
                        }

                        await unitOfWork.CommitAsync(stoppingToken);
                        _logger.LogInformation("Eventos ({Count}) marcados como finalizados com sucesso.", endedEvents.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao executar o Job de finalização de eventos.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
