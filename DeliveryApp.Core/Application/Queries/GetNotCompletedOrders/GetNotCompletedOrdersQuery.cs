using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;

public class GetNotCompletedOrdersQuery : IRequest<GetNotCompletedOrdersResponse>;
