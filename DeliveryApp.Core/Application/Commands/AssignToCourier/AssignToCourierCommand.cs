using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCourier;

/// <summary>
///     Назначить заказ на курьера
/// </summary>
public class AssignToCourierCommand : IRequest<UnitResult<Error>>;
