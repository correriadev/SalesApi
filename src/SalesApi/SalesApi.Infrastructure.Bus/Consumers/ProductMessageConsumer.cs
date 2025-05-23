using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesApi.Domain.Messages;

namespace SalesApi.Infrastructure.Bus.Consumers;

public class ProductMessageConsumer : IHandleMessages<CreateProductMessage>, IHandleMessages<ProductCreatedMessage>
{
    private readonly ILogger<ProductMessageConsumer> _logger;

    public ProductMessageConsumer(ILogger<ProductMessageConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CreateProductMessage message)
    {
        _logger.LogInformation(
            "Received CreateProductMessage - Id: {Id}, Title: {Title}, Price: {Price}",
            message.Id,
            message.Title,
            message.Price);

        await Task.CompletedTask;
    }

    public async Task Handle(ProductCreatedMessage message)
    {
        _logger.LogInformation(
            "Received ProductCreatedMessage - Id: {Id}, Success: {Success}, Message: {Message}",
            message.Id,
            message.Success,
            message.Message);

        await Task.CompletedTask;
    }
} 