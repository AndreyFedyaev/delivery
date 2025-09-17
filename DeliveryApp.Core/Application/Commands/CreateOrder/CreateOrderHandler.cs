using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Ctr
        /// </summary>
        public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<UnitResult<Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
        {
            //создаём рандомную локацию (временно)
            var randomLocation = Location.CreateRandom();

            //создаём заказ
            var orderCreateResult = Order.Create(message.OrderId, randomLocation, message.Volume);
            if (orderCreateResult.IsFailure) return orderCreateResult.Error;

            //сохраняем в репозиторий
            await _orderRepository.AddAsync(orderCreateResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
    }
}
