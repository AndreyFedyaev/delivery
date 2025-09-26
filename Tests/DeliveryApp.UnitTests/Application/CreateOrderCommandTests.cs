using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application
{
    public class CreateOrderCommandTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IOrderRepository _orderRepositoryMock = Substitute.For<IOrderRepository>();
        //private readonly ICourierRepository _courierRepositoryMock = Substitute.For<ICourierRepository>();

        [Fact]
        public async Task ReturnTrueWhenOrderIsCorrect()
        {
            //Arrange
            //var orderId = Guid.NewGuid();
            //_unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));
            //_orderRepositoryMock.AddAsync(Arg.Any<Order>()).Returns(Task.FromResult(true));

            //var createOrderCommandResult = CreateOrderCommand.Create(orderId, "Lenina", 5);
            //createOrderCommandResult.IsSuccess.Should().BeTrue();

            //var command = createOrderCommandResult.Value;
            //var handler = new CreateOrderHandler(_unitOfWork, _orderRepositoryMock);

            ////Act
            //var result = await handler.Handle(command, new CancellationToken());

            ////Assert
            //result.IsSuccess.Should().BeTrue();
        }

    }
}