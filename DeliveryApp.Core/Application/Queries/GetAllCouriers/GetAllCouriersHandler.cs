using DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers
{
    public class GetAllCouriersHandler : IRequestHandler<GetAllCouriersQuery, GetAllCouriersQueryResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;

        /// <summary>
        ///     Ctr
        /// </summary>
        public GetAllCouriersHandler(IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        }

        public async Task<GetAllCouriersQueryResponse> Handle(GetAllCouriersQuery message, CancellationToken cancellationToken)
        {
            //Выгружаем из репозитория все назначенные заказы
            var allAssignedOrders = _orderRepository.GetAllInAssignedStatus();
            if (allAssignedOrders == null || !allAssignedOrders.Any()) throw new ArgumentNullException(nameof(allAssignedOrders));

            //список всех занятых курьеров
            List<CourierDTO> allCouriers = new List<CourierDTO>();

            //Перебираем все назначенные заказы
            foreach (var order in allAssignedOrders)
            {
                //Определяем назначенного курьера
                var courier = await _courierRepository.GetAsync(order.CourierId.Value);
                if (courier.HasNoValue) throw new ArgumentNullException(nameof(courier));

                //определяем локацию курьера
                var courierLocation = new LocationDTO() { X = courier.Value.Location.X, Y = courier.Value.Location.Y };

                //добавляем курьера в список
                allCouriers.Add(new CourierDTO
                {
                    Id = courier.Value.Id,
                    Name = courier.Value.Name,
                    Location = courierLocation
                });
            }

            return new GetAllCouriersQueryResponse(allCouriers);
        }
    }
}
