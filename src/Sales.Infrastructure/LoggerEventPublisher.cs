using System.Text.Json;
using DeveloperStore.Sales.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace DeveloperStore.Sales.Infrastructure;

public class LoggerEventPublisher : IEventPublisher
{
    private readonly ILogger<LoggerEventPublisher> _logger;

    public LoggerEventPublisher(ILogger<LoggerEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync(string eventName, object payload, CancellationToken ct = default)
    {
        _logger.LogInformation("Event {Event} => {Payload}", eventName, JsonSerializer.Serialize(payload));
        return Task.CompletedTask;
    }
}
