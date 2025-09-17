using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCourier;

/// <summary>
///     Переместить курьера
/// </summary>
public class MoveCourierCommand : IRequest<UnitResult<Error>>;
