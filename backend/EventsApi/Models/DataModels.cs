using System.ComponentModel.DataAnnotations;

namespace EventsApi.Models;

public class Event
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public string Date { get; set; } = string.Empty; // ISO 8601 date string
    
    [Required]
    public string StartTime { get; set; } = string.Empty; // HH:mm format
    
    public string CreatedAt { get; set; } = string.Empty;
    
    public string UpdatedAt { get; set; } = string.Empty;
}

public class Registration
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string EventId { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? Pronouns { get; set; }
    
    public bool OptInCommunication { get; set; }
    
    public string RegisteredAt { get; set; } = string.Empty;
}

public class CreateEventRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public string Date { get; set; } = string.Empty;
    
    [Required]
    public string StartTime { get; set; } = string.Empty;
}

public class UpdateEventRequest
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Date { get; set; }
    public string? StartTime { get; set; }
}

public class CreateRegistrationRequest
{
    [Required]
    public string EventId { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string? Pronouns { get; set; }
    
    public bool OptInCommunication { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}

public class EventListResponse
{
    public List<Event> Events { get; set; } = new();
    public int Total { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}