using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.MoveCourier;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.AssignToCourier
{
    public class AssignToCourierHandler : IRequestHandler<AssignToCourierCommand, UnitResult<Error>>
    {
        private readonly IDispatchService _dispatchService;
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        ///     Ctr
        /// </summary>
        public AssignToCourierHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository, ICourierRepository courierRepository, IDispatchService dispatchService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
            _dispatchService = dispatchService ?? throw new ArgumentNullException(nameof(dispatchService));
        }

        public async Task<UnitResult<Error>> Handle(AssignToCourierCommand message, CancellationToken cancellationToken)
        {
            //Выгружаем из репозитория один неназначенный заказ
            var createdOrder = await _orderRepository.GetFirstInCreatedStatusAsync();
            if (createdOrder == null) return GeneralErrors.ValueIsRequired(nameof(createdOrder));
            var order = createdOrder.Value;

            //Выгружаем из репозитория всех свободных курьеров
            var allFreeCouriers = _courierRepository.GetAllFreeCouriers().ToList();
            if (allFreeCouriers == null || !allFreeCouriers.Any()) return GeneralErrors.ValueIsRequired(nameof(allFreeCouriers));

            //Определяем самого подходящего курьера
            var dispatchResult = _dispatchService.Dispatch(order, allFreeCouriers);
            if (dispatchResult.IsFailure) return dispatchResult;
            var courier = dispatchResult.Value;

            //сохраняем изменения
            _orderRepository.Update(order);
            _courierRepository.Update(courier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
    }
}
