using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCourier
{
    public class MoveCourierHandler : IRequestHandler<MoveCourierCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Ctr
        /// </summary>
        public MoveCourierHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<UnitResult<Error>> Handle(MoveCourierCommand message, CancellationToken cancellationToken)
        {
            //Выгружаем из репозитория все назначенные заказы
            var allAssignedOrders = _orderRepository.GetAllInAssignedStatus().ToList();
            if (allAssignedOrders == null || !allAssignedOrders.Any()) return GeneralErrors.ValueIsRequired(nameof(allAssignedOrders));

            //Перебираем все назначенные заказы
            foreach (var order in allAssignedOrders)
            {
                if (order.CourierId == null)
                    return GeneralErrors.ValueIsInvalid(nameof(order.CourierId));

                //Определяем назначенного курьера
                var courier = await _courierRepository.GetAsync((Guid)order.CourierId.Value);
                if (courier.HasNoValue) return GeneralErrors.ValueIsRequired(nameof(courier));

                //проверяем на совпадение координат курьера и заказа
                var stepsToOrders = courier.Value.CalculateTimeToLocation(order.Location);
                if (stepsToOrders.IsFailure) return stepsToOrders.Error;
                if (stepsToOrders.Value > 0)
                {
                    //Двигаем курьера на один шаг в сторону заказа
                    courier.Value.Move(order.Location);

                    //сохраняем изменения
                    _courierRepository.Update(courier.Value);
                }
                else
                {
                    //завершаем заказ
                    order.Complete();

                    //освобождаем курьера
                    courier.Value.ComplateOrder(order);

                    //сохраняем изменения
                    _orderRepository.Update(order);
                    _courierRepository.Update(courier.Value);
                }
            }

            //сохраняем репозиторий
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
    }
}
