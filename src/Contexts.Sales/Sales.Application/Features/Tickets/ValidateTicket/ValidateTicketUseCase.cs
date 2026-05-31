using Sales.Application.DTOs;
using Sales.Domain.Repositories;
using TicketApi.Common.Exceptions;

namespace Sales.Application.Features.Tickets.ValidateTicket;

public class ValidateTicketUseCase
{
    private readonly ITicketRepository _ticketRepository;

    public ValidateTicketUseCase(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketDto> ExecuteAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("INVALID_CODE", "O código do ingresso não pode ser vazio.");

        var ticket = await _ticketRepository.GetByCodeAsync(code, cancellationToken);
        if (ticket == null)
            throw new DomainException("TICKET_NOT_FOUND", "Ingresso não encontrado.");

        ticket.Use();
        await _ticketRepository.SaveChangesAsync(cancellationToken);

        return new TicketDto(
            ticket.Id,
            ticket.OrderItemId,
            ticket.Code,
            ticket.Status.ToString().ToLower()
        );
    }
}
