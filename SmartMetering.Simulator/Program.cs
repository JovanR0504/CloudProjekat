using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var apiBaseUrl = configuration["Simulator:ApiBaseUrl"] ?? "http://localhost:7071";
var deviceId = configuration["Simulator:DeviceId"] ?? "placeholder-device-id";
var deviceToken = configuration["Simulator:DeviceToken"] ?? "placeholder-token";
var intervalSeconds = int.Parse(configuration["Simulator:IntervalSeconds"] ?? "5");

Console.WriteLine("=== SmartMetering Device Simulator ===");
Console.WriteLine($"API Base URL : {apiBaseUrl}");
Console.WriteLine($"Device ID    : {deviceId}");
Console.WriteLine($"Interval     : {intervalSeconds}s");
Console.WriteLine("======================================");
Console.WriteLine();

using var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(apiBaseUrl);
httpClient.DefaultRequestHeaders.Add("X-Device-Id", deviceId);
httpClient.DefaultRequestHeaders.Add("X-Device-Token", deviceToken);

var random = new Random();
var totalKwh = 100.0 + random.NextDouble() * 50;

while (true)
{
    try
    {
        var voltage = 220.0 + random.NextDouble() * 20.0;
        var current = 1.0 + random.NextDouble() * 24.0;
        var powerFactor = 0.85 + random.NextDouble() * 0.14;
        var loadKw = voltage * current * powerFactor / 1000.0;
        totalKwh += loadKw * intervalSeconds / 3600.0;

        var payload = new
        {
            Voltage = Math.Round(voltage, 2),
            Current = Math.Round(current, 2),
            PowerFactor = Math.Round(powerFactor, 4),
            TotalKwh = Math.Round(totalKwh, 4),
            LoadKw = Math.Round(loadKw, 4),
            RecordedAtUtc = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("/api/telemetry", content);

        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] Sent: U={payload.Voltage}V, I={payload.Current}A, " +
            $"PF={payload.PowerFactor}, Load={payload.LoadKw}kW, Total={payload.TotalKwh}kWh " +
            $"-> {(int)response.StatusCode} {response.ReasonPhrase}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error: {ex.Message}");
    }

    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));
}
