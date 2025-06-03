using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using EventsApi.Models;
using EventsApi.Services;
using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace EventsApi.Functions;

public class EventFunctions
{
    private readonly ILogger _logger;
    private readonly IEventService _eventService;

    public EventFunctions(ILoggerFactory loggerFactory, IEventService eventService)
    {
        _logger = loggerFactory.CreateLogger<EventFunctions>();
        _eventService = eventService;
    }

    [Function("CreateEvent")]
    public async Task<HttpResponseData> CreateEvent([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "events")] HttpRequestData req)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createRequest = JsonSerializer.Deserialize<CreateEventRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (createRequest == null)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.BadRequest, 
                    new ApiResponse<object> { Success = false, Error = "Invalid request body" });
            }

            // Validate the request
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(createRequest);
            if (!Validator.TryValidateObject(createRequest, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                return await CreateJsonResponseAsync(req, HttpStatusCode.BadRequest,
                    new ApiResponse<object> { Success = false, Error = $"Validation failed: {errors}" });
            }

            var eventItem = await _eventService.CreateEventAsync(createRequest);
            
            return await CreateJsonResponseAsync(req, HttpStatusCode.Created,
                new ApiResponse<Event> { Success = true, Data = eventItem, Message = "Event created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return await CreateJsonResponseAsync(req, HttpStatusCode.InternalServerError,
                new ApiResponse<object> { Success = false, Error = "Internal server error" });
        }
    }

    [Function("GetEvent")]
    public async Task<HttpResponseData> GetEvent([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "events/{id}")] HttpRequestData req, string id)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            var eventItem = await _eventService.GetEventAsync(id);
            
            if (eventItem == null)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.NotFound,
                    new ApiResponse<object> { Success = false, Error = "Event not found" });
            }

            return await CreateJsonResponseAsync(req, HttpStatusCode.OK,
                new ApiResponse<Event> { Success = true, Data = eventItem });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event {EventId}", id);
            return await CreateJsonResponseAsync(req, HttpStatusCode.InternalServerError,
                new ApiResponse<object> { Success = false, Error = "Internal server error" });
        }
    }

    [Function("GetEvents")]
    public async Task<HttpResponseData> GetEvents([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "events")] HttpRequestData req)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var date = query["date"];
            var location = query["location"];
            var limit = int.TryParse(query["limit"], out var l) ? l : 10;
            var offset = int.TryParse(query["offset"], out var o) ? o : 0;

            var events = await _eventService.GetEventsAsync(date, location, limit, offset);
            
            return await CreateJsonResponseAsync(req, HttpStatusCode.OK,
                new ApiResponse<EventListResponse> { Success = true, Data = events });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events");
            return await CreateJsonResponseAsync(req, HttpStatusCode.InternalServerError,
                new ApiResponse<object> { Success = false, Error = "Internal server error" });
        }
    }

    [Function("UpdateEvent")]
    public async Task<HttpResponseData> UpdateEvent([HttpTrigger(AuthorizationLevel.Anonymous, "put", "options", Route = "events/{id}")] HttpRequestData req, string id)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updateRequest = JsonSerializer.Deserialize<UpdateEventRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updateRequest == null)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.BadRequest,
                    new ApiResponse<object> { Success = false, Error = "Invalid request body" });
            }

            var eventItem = await _eventService.UpdateEventAsync(id, updateRequest);
            
            if (eventItem == null)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.NotFound,
                    new ApiResponse<object> { Success = false, Error = "Event not found" });
            }

            return await CreateJsonResponseAsync(req, HttpStatusCode.OK,
                new ApiResponse<Event> { Success = true, Data = eventItem, Message = "Event updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", id);
            return await CreateJsonResponseAsync(req, HttpStatusCode.InternalServerError,
                new ApiResponse<object> { Success = false, Error = "Internal server error" });
        }
    }

    [Function("DeleteEvent")]
    public async Task<HttpResponseData> DeleteEvent([HttpTrigger(AuthorizationLevel.Anonymous, "delete", "options", Route = "events/{id}")] HttpRequestData req, string id)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            var deleted = await _eventService.DeleteEventAsync(id);
            
            if (!deleted)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.NotFound,
                    new ApiResponse<object> { Success = false, Error = "Event not found" });
            }

            return await CreateJsonResponseAsync(req, HttpStatusCode.OK,
                new ApiResponse<object> { Success = true, Message = "Event deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            return await CreateJsonResponseAsync(req, HttpStatusCode.InternalServerError,
                new ApiResponse<object> { Success = false, Error = "Internal server error" });
        }
    }

    private HttpResponseData CreateCorsResponse(HttpRequestData req, HttpStatusCode statusCode)
    {
        var response = req.CreateResponse(statusCode);
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        return response;
    }

    private async Task<HttpResponseData> CreateJsonResponseAsync<T>(HttpRequestData req, HttpStatusCode statusCode, T data)
    {
        var response = req.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json");
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        
        await response.WriteStringAsync(JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
        
        return response;
    }
}