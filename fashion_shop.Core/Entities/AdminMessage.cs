using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fashion_shop.API.ExternalService.Entities;

public class AdminMessage
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string? RedirectUrl { get; set; }
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public bool IsRead { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
}

public class AdminMessageQueue
{
    public string? HandledBy { get; set; }
    public string? RedirectUrl { get; set; }
    public string Title { get; set; } = default!;
    public string Body { get; set; } = default!;
    public bool IsRead { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; }
}

