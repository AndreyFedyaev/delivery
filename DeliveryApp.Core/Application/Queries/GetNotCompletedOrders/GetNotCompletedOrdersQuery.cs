using MediatR;

namespace DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;

public class GetNotCompletedOrdersQuery : IRequest<GetNotCompletedOrdersResponse>;
