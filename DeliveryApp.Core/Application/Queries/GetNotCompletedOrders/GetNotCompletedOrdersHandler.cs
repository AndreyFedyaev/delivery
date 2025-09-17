using DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders
{
    public class GetNotCompletedOrdersHandler : IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>
    {
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        ///     Ctr
        /// </summary>
        public GetNotCompletedOrdersHandler(IOrderRepository orderRepository, ICourierRepository courierRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<GetNotCompletedOrdersResponse> Handle(GetNotCompletedOrdersQuery message, CancellationToken cancellationToken)
        {
            //Выгружаем из репозитория все назначенные заказы
            var allAssignedOrders = _orderRepository.GetAllInAssignedStatus();
            if (allAssignedOrders == null || !allAssignedOrders.Any()) throw new ArgumentNullException(nameof(allAssignedOrders));

            //список всех назначенных заказов
            List<OrderDTO> orders = new List<OrderDTO>();

            //Перебираем все назначенные заказы
            foreach (var order in allAssignedOrders)
            {
                //определяем локацию заказа
                var orderLocation = new LocationDTO() { X = order.Location.X, Y = order.Location.Y };

                //добавляем заказ в список
                orders.Add(new OrderDTO
                {
                    Id = order.Id,
                    Location = orderLocation
                });
            }

            return new GetNotCompletedOrdersResponse(orders);
        }
    }
}
