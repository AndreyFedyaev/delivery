using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services
{
    public class DispatchServiceTests
    {
        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnDispatch()
        {
            //Arrange
            var location1 = Location.Create(1, 1).Value;
            var location2 = Location.Create(2, 2).Value;
            var location3 = Location.Create(3, 3).Value;

            var courier1 = Courier.Create("Jec", 2, location1);
            var courier2 = Courier.Create("Bob", 2, location2);
            var courier3 = Courier.Create("Tom", 2, location3);

            List<Courier> couriers = [courier1.Value, courier2.Value, courier3.Value];

            var orderLocation = Location.Create(5, 5).Value;
            var order = Order.Create(Guid.NewGuid(), orderLocation, 5);

            //Act
            var dispatchService = new DispatchService();
            var result = dispatchService.Dispatch(order.Value, couriers);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
