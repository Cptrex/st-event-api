﻿using ST.Events.API.Interfaces;

namespace ST.Events.API.Models;

public class EventSingleton : IEventSingleton
{
    public List<User> Users { get; set; } = new()
    {
        new User(
            new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 
            "User1", 
            new List<Ticket>
            {
                new (new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 
                    new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    0,
                    0)
            })
    };
}