using System;
using System.Threading;
using System.Threading.Tasks;
using Sales.Application.DTOs;
using Sales.Domain.Exceptions;
using Sales.Domain.Repositories;

namespace Sales.Application.UseCases;

public class ValidateTicketUseCase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ValidateTicketUseCase(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketDto> ExecuteAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("O código do ingresso não pode ser vazio.", nameof(code));

        var ticket = await _ticketRepository.GetByCodeAsync(code, cancellationToken);
        if (ticket == null)
            throw new TicketNotFoundException();

        ticket.Use();
        _ticketRepository.Update(ticket);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new TicketDto(
            ticket.Id,
            ticket.OrderItemId,
            ticket.Code,
            ticket.Status.ToString().ToLower()
        );
    }
}
