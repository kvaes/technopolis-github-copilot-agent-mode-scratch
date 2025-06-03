using EventsApi.Models;
using System.Collections.Concurrent;

namespace EventsApi.Services;

public interface IEventService
{
    Task<Event> CreateEventAsync(CreateEventRequest request);
    Task<Event?> GetEventAsync(string id);
    Task<EventListResponse> GetEventsAsync(string? date = null, string? location = null, int limit = 10, int offset = 0);
    Task<Event?> UpdateEventAsync(string id, UpdateEventRequest request);
    Task<bool> DeleteEventAsync(string id);
}

public interface IRegistrationService
{
    Task<Registration> CreateRegistrationAsync(CreateRegistrationRequest request);
    Task<List<Registration>> GetRegistrationsByEventAsync(string eventId);
}

public class InMemoryEventService : IEventService
{
    private readonly ConcurrentDictionary<string, Event> _events = new();

    public Task<Event> CreateEventAsync(CreateEventRequest request)
    {
        var eventItem = new Event
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Location = request.Location,
            Date = request.Date,
            StartTime = request.StartTime,
            CreatedAt = DateTime.UtcNow.ToString("O"),
            UpdatedAt = DateTime.UtcNow.ToString("O")
        };

        _events[eventItem.Id] = eventItem;
        return Task.FromResult(eventItem);
    }

    public Task<Event?> GetEventAsync(string id)
    {
        _events.TryGetValue(id, out var eventItem);
        return Task.FromResult(eventItem);
    }

    public Task<EventListResponse> GetEventsAsync(string? date = null, string? location = null, int limit = 10, int offset = 0)
    {
        var events = _events.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(date))
        {
            events = events.Where(e => e.Date == date);
        }

        if (!string.IsNullOrEmpty(location))
        {
            events = events.Where(e => e.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
        }

        var total = events.Count();
        var filteredEvents = events
            .OrderBy(e => e.Date)
            .ThenBy(e => e.StartTime)
            .Skip(offset)
            .Take(limit)
            .ToList();

        return Task.FromResult(new EventListResponse
        {
            Events = filteredEvents,
            Total = total,
            Offset = offset,
            Limit = limit
        });
    }

    public Task<Event?> UpdateEventAsync(string id, UpdateEventRequest request)
    {
        if (!_events.TryGetValue(id, out var eventItem))
        {
            return Task.FromResult<Event?>(null);
        }

        if (!string.IsNullOrEmpty(request.Name))
            eventItem.Name = request.Name;
        
        if (!string.IsNullOrEmpty(request.Location))
            eventItem.Location = request.Location;
        
        if (!string.IsNullOrEmpty(request.Date))
            eventItem.Date = request.Date;
        
        if (!string.IsNullOrEmpty(request.StartTime))
            eventItem.StartTime = request.StartTime;

        eventItem.UpdatedAt = DateTime.UtcNow.ToString("O");
        
        return Task.FromResult<Event?>(eventItem);
    }

    public Task<bool> DeleteEventAsync(string id)
    {
        return Task.FromResult(_events.TryRemove(id, out _));
    }
}

public class InMemoryRegistrationService : IRegistrationService
{
    private readonly ConcurrentDictionary<string, Registration> _registrations = new();

    public Task<Registration> CreateRegistrationAsync(CreateRegistrationRequest request)
    {
        var registration = new Registration
        {
            Id = Guid.NewGuid().ToString(),
            EventId = request.EventId,
            Name = request.Name,
            Email = request.Email,
            Pronouns = request.Pronouns,
            OptInCommunication = request.OptInCommunication,
            RegisteredAt = DateTime.UtcNow.ToString("O")
        };

        _registrations[registration.Id] = registration;
        return Task.FromResult(registration);
    }

    public Task<List<Registration>> GetRegistrationsByEventAsync(string eventId)
    {
        var registrations = _registrations.Values
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.RegisteredAt)
            .ToList();

        return Task.FromResult(registrations);
    }
}