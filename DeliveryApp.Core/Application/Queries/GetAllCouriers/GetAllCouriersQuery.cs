using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;

public class GetAllCouriersQuery : IRequest<GetAllCouriersQueryResponse>;
