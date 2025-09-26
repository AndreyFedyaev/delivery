using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGeoServiceClient _geoServiceClient;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Ctr
        /// </summary>
        public CreateOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, IGeoServiceClient geoServiceClient)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _geoServiceClient = geoServiceClient ?? throw new ArgumentNullException(nameof(geoServiceClient));
        }

        public async Task<UnitResult<Error>> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
        {
            //создаём рандомную локацию (временно)
            var orderLocation = _geoServiceClient.GetGeolocationAsync(message.Street, cancellationToken);

            //создаём заказ
            var orderCreateResult = Order.Create(message.OrderId, orderLocation.Result.Value, message.Volume);
            if (orderCreateResult.IsFailure) return orderCreateResult.Error;

            //сохраняем в репозиторий
            await _orderRepository.AddAsync(orderCreateResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
    }
}
