using Sales.Domain.Enums;
using Sales.Domain.Repositories;

namespace Sales.API.Jobs;

public class ReservationExpiryJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReservationExpiryJob> _logger;

    public ReservationExpiryJob(IServiceProvider serviceProvider, ILogger<ReservationExpiryJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Job de expiração de reservas iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var reservationRepository = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                    var ticketTypeRepository = scope.ServiceProvider.GetRequiredService<ITicketTypeRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var now = DateTime.UtcNow;
                    var expiredReservations = await reservationRepository.GetExpiredActiveReservationsAsync(now, stoppingToken);

                    if (expiredReservations.Count > 0)
                    {
                        _logger.LogInformation("Encontradas {Count} reservas expiradas para processar.", expiredReservations.Count);

                        foreach (var reservation in expiredReservations)
                        {
                            reservation.Expire();
                            reservationRepository.Update(reservation);

                            var order = await orderRepository.GetByIdAsync(reservation.OrderId, stoppingToken);
                            if (order != null && order.Status == OrderStatus.Pending)
                            {
                                order.Cancel();
                                orderRepository.Update(order);

                                var ticketType = await ticketTypeRepository.GetByIdAsync(reservation.TicketTypeId, stoppingToken);
                                if (ticketType != null)
                                {
                                    ticketType.RestoreStock(reservation.Quantity);
                                    ticketTypeRepository.Update(ticketType);
                                }
                            }
                        }

                        await unitOfWork.CommitAsync(stoppingToken);
                        _logger.LogInformation("Reservas ({Count}) expiradas com sucesso e estoque liberado.", expiredReservations.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao executar o Job de expiração de reservas.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
