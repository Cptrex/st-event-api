﻿using MongoDB.Driver;
using ST.Events.API.Interfaces;
using System.Text;
using Newtonsoft.Json;
using SC.Internship.Common.ScResult;
using ST.Events.API.Enumerics;

namespace ST.Events.API.Features.Ticket;

public class TicketManager : ITicketManager
{
    private readonly IMongoCollection<Models.Event> _eventContext;
    private readonly HttpClient _httpPaymentServiceClient;

    public TicketManager(IHttpClientFactory httpClientFactory, IMongoDBCommunicator mongoDb)
    {
        _eventContext = mongoDb.DbCollection;
        _httpPaymentServiceClient = httpClientFactory.CreateClient("paymentService");
    }

    public async Task<List<Models.Ticket>> GetAllEventTickets(Guid eventId)
    {
        var foundEvent = (await _eventContext.Find(e => e.Id == eventId).ToListAsync()).First();

        return foundEvent.Tickets;
    }

    public async Task<int> GetLastTicketNumberInEvent(Guid eventId)
    {
        var tickets = await GetAllEventTickets(eventId);
        var lastTickerNumber = tickets.OrderBy(t => t.PlaceNumber).ToList().Last().PlaceNumber;

        return lastTickerNumber;
    }

    public async Task<Guid> CreateTicketPayment()
    {
        HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        var result = await _httpPaymentServiceClient.PostAsync($"{_httpPaymentServiceClient.BaseAddress}payments", content);

        var response = JsonConvert.DeserializeObject<ScResult<Guid>>(await result.Content.ReadAsStringAsync());

        if (response is null) return Guid.Empty;

        var paymentTransactionId = response.Result;

        return paymentTransactionId;
    }

    public async Task CancelTicketPayment(Guid createdTransactionId)
    {
        HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        await _httpPaymentServiceClient.PutAsync($"{_httpPaymentServiceClient.BaseAddress}payments/{createdTransactionId}/{PaymentState.Canceled}", content);
    }

    public async Task ConfirmTicketPayment(Guid createdTransactionId)
    {
        HttpContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        await _httpPaymentServiceClient.PutAsync($"{_httpPaymentServiceClient.BaseAddress}payments/{createdTransactionId}/{PaymentState.Confirmed}", content);
    }
}