using System;

namespace Sales.Domain.Entities;

public class Event
{
    public Guid Id { get; private set; }
    public string Status { get; private set; } = null!;
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }

    // EF Core constructor
    private Event() { }

    public bool IsActive(DateTime now)
    {
        return Status.Equals("published", StringComparison.OrdinalIgnoreCase) && now < StartsAt;
    }

    public void Finish()
    {
        Status = "finished";
    }
}
