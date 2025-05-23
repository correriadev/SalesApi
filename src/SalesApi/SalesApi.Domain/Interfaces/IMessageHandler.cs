namespace SalesApi.Domain.Interfaces;

public interface IMessageHandler<TMessage>
{
    Task Handle(TMessage message);
} 