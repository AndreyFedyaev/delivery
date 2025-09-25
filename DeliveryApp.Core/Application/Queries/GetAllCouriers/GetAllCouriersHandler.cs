using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.Queries.GetAllCouriers
{
    public class GetAllCouriersHandler : IRequestHandler<GetAllCouriersQuery, GetAllCouriersQueryResponse>
    {
        private readonly string _connectionString;

        public GetAllCouriersHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<GetAllCouriersQueryResponse> Handle(GetAllCouriersQuery message, CancellationToken cancellationToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT id, name, location_x, location_y FROM public.couriers"
                , new { });

            if (result.AsList().Count == 0)
                return null;

            var couriers = new List<CourierDTO>();
            foreach (var item in result)
            {
                var location = new LocationDTO() { X = item.location_x, Y = item.location_y };
                var courier = new CourierDTO { Id = item.id, Name = item.name, Location = location };

                couriers.Add(courier);
            }

            return new GetAllCouriersQueryResponse(couriers);

   
        }
    }
}
