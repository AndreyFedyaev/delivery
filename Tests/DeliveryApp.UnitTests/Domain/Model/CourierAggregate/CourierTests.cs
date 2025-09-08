using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierTests
    {
        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnCreated()
        {
            //Arrange
            var location = Location.Create(1, 1).Value;

            //Act
            var courier = Courier.Create("Alex", 2, location);

            //Assert
            courier.IsSuccess.Should().BeTrue();
            courier.Value.Name.Should().Be("Alex");
            courier.Value.Speed.Should().Be(2);
            courier.Value.Location.Should().Be(location);
        }

        [Theory]
        [InlineData("Alex", 0)]
        [InlineData("", 2)]
        [InlineData("", 0)]
        public void BeCorrectWhenParamsAreCorrectOnCanCreated(string name, int speed)
        {
            //Arrange
            var location = Location.Create(1, 1).Value;

            //Act
            var courier = Courier.Create(name, speed, location);

            //Assert
            courier.IsSuccess.Should().BeFalse();
            courier.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnAddStoragePlace()
        {
            //Arrange
            var location = Location.Create(1, 1).Value;
            var courier = Courier.Create("Alex", 2, location);

            //Act
            var addStoragePlaceResult = courier.Value.AddStoragePlace("Box", 20);

            //Assert
            courier.IsSuccess.Should().BeTrue();
            bool count = courier.Value.StoragePlaces.Count > 1;
        }

        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnCanTakeOrder()
        {
            //Arrange
            var courierLocation = Location.Create(1, 1).Value;
            var orderLocation = Location.Create(5, 5).Value;

            var courier = Courier.Create("Alex", 2, courierLocation);
            var addStoragePlaceResult = courier.Value.AddStoragePlace("Box", 20);

            var order = Order.Create(Guid.NewGuid(), orderLocation, 15);

            //Act
            var canTakeOrder = courier.Value.CanTakeOrder(order.Value);

            //Assert
            courier.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnTakeOrder()
        {
            //Arrange
            var courierLocation = Location.Create(1, 1).Value;
            var orderLocation = Location.Create(5, 5).Value;

            var courier = Courier.Create("Alex", 2, courierLocation);
            var addStoragePlaceResult = courier.Value.AddStoragePlace("Box", 20);

            var order = Order.Create(Guid.NewGuid(), orderLocation, 15);

            var canTakeOrder = courier.Value.CanTakeOrder(order.Value);

            //Act
            var takeOrder = courier.Value.TakeOrder(order.Value);

            //Assert
            courier.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnComplateOrder()
        {
            //Arrange
            var courierLocation = Location.Create(1, 1).Value;
            var orderLocation = Location.Create(5, 5).Value;

            var courier = Courier.Create("Alex", 2, courierLocation);
            var addStoragePlaceResult = courier.Value.AddStoragePlace("Box", 20);

            var order = Order.Create(Guid.NewGuid(), orderLocation, 15);

            var canTakeOrder = courier.Value.CanTakeOrder(order.Value);
            var takeOrder = courier.Value.TakeOrder(order.Value);

            //Act
            var complateOrder = courier.Value.ComplateOrder(order.Value);

            //Assert
            courier.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnCalculateTimeToLocation()
        {
            //Arrange
            var courierLocation = Location.Create(1, 1).Value;
            var orderLocation = Location.Create(5, 5).Value;

            var courier = Courier.Create("Alex", 2, courierLocation);
            var addStoragePlaceResult = courier.Value.AddStoragePlace("Box", 20);

            var order = Order.Create(Guid.NewGuid(), orderLocation, 15);

            var canTakeOrder = courier.Value.CanTakeOrder(order.Value);
            var takeOrder = courier.Value.TakeOrder(order.Value);

            //Act
            var calculateTimeToLocation = courier.Value.CalculateTimeToLocation(orderLocation);

            //Assert
            courier.IsSuccess.Should().BeTrue();
            var stepCount = calculateTimeToLocation.Value;
        }


    }
}
