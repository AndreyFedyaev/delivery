using DeliveryApp.Core.Domain.Model.CourierAggregate;
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
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(20)]
        public void BeCorrectWhenParamsAreCorrectOnCanStore(int volume)
        {
            //Act
            var storagePlace = StoragePlace.Create("BackPack", 20);

            var canStore = storagePlace.Value.CanStore(volume);

            //Assert
            canStore.IsSuccess.Should().BeTrue();
        }


    }
}
