namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

public class GetNotCompletedOrdersResponse
{
    public GetNotCompletedOrdersResponse(List<OrderDTO> orders)
    {
        Orders.AddRange(orders);
    }

    public List<OrderDTO> Orders { get; set; } = new();
}
