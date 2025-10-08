using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

namespace DeliveryApp.Core.Ports;
public interface IMessageBusProducer
{
    Task PublishOrderCreated(OrderCreatedDomainEvent notification, CancellationToken cancellationToken);
    Task PublishOrderCompleted(OrderCompletedDomainEvent notification, CancellationToken cancellationToken);
}
