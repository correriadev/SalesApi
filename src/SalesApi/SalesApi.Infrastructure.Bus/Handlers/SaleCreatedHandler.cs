using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SalesApi.Infrastructure.Bus.Handlers;

public class SaleCreatedHandler : IMessageHandler<SaleCreated>
{
    private readonly ILogger<SaleCreatedHandler> _logger;

    public SaleCreatedHandler(ILogger<SaleCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(SaleCreated message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sale created: {SaleNumber}", message.SaleNumber);
        return Task.CompletedTask;
    }
}

public class SaleCreated
{
    public string SaleNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerId { get; set; } = string.Empty;
} 