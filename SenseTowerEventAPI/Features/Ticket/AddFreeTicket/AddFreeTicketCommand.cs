﻿using JetBrains.Annotations;
using MediatR;
using SenseTowerEventAPI.Interfaces;

namespace SenseTowerEventAPI.Features.Ticket.AddFreeTicket;

/// <summary>
/// Модель команды добавления бесплатного билета
/// </summary>
[UsedImplicitly]
public class AddFreeTicketCommand : IRequest<Guid>, ITicket, IEntity
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid Owner { get; set; }
    public int PlaceNumber { get; set; }

    public AddFreeTicketCommand(Guid id, Guid eventId, Guid owner, int place)
    {
        Id = id;
        EventId= eventId;
        Owner = owner;
        PlaceNumber = place;
    }
}