namespace DeliveryApp.Core.Application.Queries.GetAllCouriers
{
    public class GetAllCouriersQueryResponse
    {
        public GetAllCouriersQueryResponse(List<CourierDTO> couriers)
        {
            Couriers.AddRange(couriers);
        }

        public List<CourierDTO> Couriers { get; set; } = new();
    }
}
