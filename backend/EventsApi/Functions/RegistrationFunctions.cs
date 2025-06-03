using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using EventsApi.Models;
using EventsApi.Services;
using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace EventsApi.Functions;

public class RegistrationFunctions
{
    private readonly ILogger _logger;
    private readonly IRegistrationService _registrationService;
    private readonly IEventService _eventService;

    public RegistrationFunctions(ILoggerFactory loggerFactory, IRegistrationService registrationService, IEventService eventService)
    {
        _logger = loggerFactory.CreateLogger<RegistrationFunctions>();
        _registrationService = registrationService;
        _eventService = eventService;
    }

    [Function("CreateRegistration")]
    public async Task<HttpResponseData> CreateRegistration([HttpTrigger(AuthorizationLevel.Anonymous, "post", "options", Route = "registrations")] HttpRequestData req)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createRequest = JsonSerializer.Deserialize<CreateRegistrationRequest>(requestBody, new JsonSerializerOptions
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

            // Check if event exists
            var eventItem = await _eventService.GetEventAsync(createRequest.EventId);
            if (eventItem == null)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.BadRequest,
                    new ApiResponse<object> { Success = false, Error = "Event not found" });
            }

            var registration = await _registrationService.CreateRegistrationAsync(createRequest);

            return await CreateJsonResponseAsync(req, HttpStatusCode.Created,
                new ApiResponse<Registration> { Success = true, Data = registration, Message = "Registration created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating registration");
            return await CreateJsonResponseAsync(req, HttpStatusCode.InternalServerError,
                new ApiResponse<object> { Success = false, Error = "Internal server error" });
        }
    }

    [Function("GetRegistrationsByEvent")]
    public async Task<HttpResponseData> GetRegistrationsByEvent([HttpTrigger(AuthorizationLevel.Anonymous, "get", "options", Route = "events/{eventId}/registrations")] HttpRequestData req, string eventId)
    {
        // Handle CORS preflight
        if (req.Method == "OPTIONS")
        {
            return CreateCorsResponse(req, HttpStatusCode.OK);
        }

        try
        {
            // Check if event exists
            var eventItem = await _eventService.GetEventAsync(eventId);
            if (eventItem == null)
            {
                return await CreateJsonResponseAsync(req, HttpStatusCode.NotFound,
                    new ApiResponse<object> { Success = false, Error = "Event not found" });
            }

            var registrations = await _registrationService.GetRegistrationsByEventAsync(eventId);

            return await CreateJsonResponseAsync(req, HttpStatusCode.OK,
                new ApiResponse<List<Registration>> { Success = true, Data = registrations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting registrations for event {EventId}", eventId);
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