using Dapper;
using DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders
{
    public class GetNotCompletedOrdersHandler : IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>
    {
        private readonly string _connectionString;

        public GetNotCompletedOrdersHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }


        public async Task<GetNotCompletedOrdersResponse> Handle(GetNotCompletedOrdersQuery message, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT id, courier_id, location_x, location_y FROM public.orders where status!=@status;"
                , new { status = OrderStatus.Completed.Name });

            if (result.AsList().Count == 0)
                return null;

            var orders = new List<OrderDTO>();
            foreach (var item in result)
            {
                var location = new LocationDTO() { X = item.location_x, Y = item.location_y };
                var order = new OrderDTO { Id = item.id, Location = location };

                orders.Add(order);
            }

            return new GetNotCompletedOrdersResponse(orders);
        }
    }
}
