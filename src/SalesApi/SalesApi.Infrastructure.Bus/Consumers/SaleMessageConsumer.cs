using Microsoft.Extensions.Logging;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Messages;

namespace SalesApi.Infrastructure.Bus.Consumers;

public class SaleMessageConsumer : IMessageHandler<CreateSaleMessage>, IMessageHandler<SaleCreatedMessage>
{
    private readonly ILogger<SaleMessageConsumer> _logger;

    public SaleMessageConsumer(ILogger<SaleMessageConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CreateSaleMessage message)
    {
        _logger.LogInformation(
            "Received CreateSaleMessage - Id: {Id}, Customer: {CustomerName}, Items: {ItemCount}",
            message.Id,
            message.CustomerName,
            message.Items?.Count ?? 0);

        if (message.Items != null)
        {
            foreach (var item in message.Items)
            {
                _logger.LogInformation(
                    "Sale Item - ProductId: {ProductId}, Quantity: {Quantity}, UnitPrice: {UnitPrice}",
                    item.ProductId,
                    item.Quantity,
                    item.UnitPrice);
            }
        }

        await Task.CompletedTask;
    }

    public async Task Handle(SaleCreatedMessage message)
    {
        _logger.LogInformation(
            "Received SaleCreatedMessage - Id: {Id}, Success: {Success}, Message: {Message}",
            message.Id,
            message.Success,
            message.Message);

        await Task.CompletedTask;
    }
} 