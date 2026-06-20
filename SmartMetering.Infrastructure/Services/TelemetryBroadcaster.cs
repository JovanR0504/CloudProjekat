using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartMetering.Application.Interfaces;
using SmartMetering.Contracts.Telemetry;

namespace SmartMetering.Infrastructure.Services;

public class TelemetryBroadcaster : ITelemetryBroadcaster
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelemetryBroadcaster> _logger;

    public TelemetryBroadcaster(HttpClient httpClient, IConfiguration configuration,
        ILogger<TelemetryBroadcaster> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var baseUrl = configuration["Broadcast:ApiBaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
            _httpClient.BaseAddress = new Uri(baseUrl);

        var secret = configuration["Broadcast:SharedSecret"];
        if (!string.IsNullOrEmpty(secret))
            _httpClient.DefaultRequestHeaders.Add("X-Broadcast-Secret", secret);
    }

    public async Task BroadcastAsync(TelemetryBroadcastEvent evt, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/internal/broadcast", evt, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Broadcast failed for meter {MeterId} with status {StatusCode}.",
                    evt.MeterId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error broadcasting telemetry for meter {MeterId}.", evt.MeterId);
        }
    }
}
