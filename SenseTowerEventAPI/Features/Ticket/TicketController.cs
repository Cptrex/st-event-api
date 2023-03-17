﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.Internship.Common.ScResult;
using SenseTowerEventAPI.Features.Ticket.GiveTicketUser;

namespace SenseTowerEventAPI.Features.Ticket;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class TicketController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Выдать бесплатный билет пользователю
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("give-free-ticket")]
    public async Task<ScResult> GiveFreeTicketUser(GiveTicketUserCommand cmd)
    {
        await _mediator.Send(cmd);

        return new ScResult();
    }

    /// <summary>
    /// Проверить есть ли билет у пользователя на данное мероприятие
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("check-ticket-exist")]
    public async Task<ScResult> CheckUserTicketExist()
    {
        return await Task.FromResult(new ScResult());
    }
}