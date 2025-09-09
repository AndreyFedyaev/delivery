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
    public class StoragePlaceTests
    {
        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnCreated()
        {
            
            //Act
            var storagePlace = StoragePlace.Create("BackPack", 20);

            //Assert
            storagePlace.IsSuccess.Should().BeTrue();
            storagePlace.Value.Name.Should().Be("BackPack");
            storagePlace.Value.TotalVolume.Should().Be(20);
        }

        [Theory]
        [InlineData("BackPack", 0)]
        [InlineData("BackPack", -5)]
        [InlineData("", 20)]
        [InlineData(null, 20)]
        [InlineData("", 0)]
        [InlineData("", -2)]
        [InlineData(null, 0)]
        [InlineData(null, -3)]
        public void ReturnErrorWhenParamsAreNotCorrectOnCreated(string name, int volume)
        {
            //Act
            var storagePlace = StoragePlace.Create(name, volume);

            //Assert
            storagePlace.IsSuccess.Should().BeFalse();
            storagePlace.Error.Should().NotBeNull();
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
  
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(30)]
        public void ReturnErrorWhenParamsAreNotCorrectOnCanStore(int volume)
        {
            //Act
            var storagePlace = StoragePlace.Create("BackPack", 20);

            var canStore = storagePlace.Value.CanStore(volume);

            //Assert
            canStore.IsSuccess.Should().BeFalse();
            canStore.Error.Should().NotBeNull();
        }
    }
}
