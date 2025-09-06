using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;


namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel
{
    public class LocationTests
    {
        [Fact]
        public void BeCorrectWhenParamsAreCorrectOnCreated()
        {
            //Act
            var location = Location.Create(5,8);

            //Assert
            location.IsSuccess.Should().BeTrue();
            location.Value.X.Should().Be(5);
            location.Value.Y.Should().Be(8);
        }

        [Theory]
        [InlineData(0, 5)]
        [InlineData(-1, 5)]
        [InlineData(11, 5)]
        [InlineData(5, 0)]
        [InlineData(5, -1)]
        [InlineData(5, 11)]
        public void ReturnErrorWhenParamsAreNotCorrectOnCreated(int x, int y)
        {
            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var first = Location.Create(5, 8).Value;
            var second = Location.Create(5, 8).Value;

            //Act
            var result = first == second;

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeNotEqualWhenOneOfPropertiesIsNotEqual()
        {
            //Arrange
            var first = Location.Create(5, 8).Value;
            var second = Location.Create(3, 4).Value;

            //Act
            var result = first == second;

            //Assert
            result.Should().BeFalse();
        }

        //не уверен в необходимости этого
        [Fact]
        public void TestRandomCreated()
        {
            //Act
            var location = Location.CreateRandom();
        }

        [Theory]
        [InlineData(2, 3, 9, 8)]
        [InlineData(2, 6, 4, 9)]
        [InlineData(9, 9, 2, 2)]
        public void BeCorrectWhenParamsAreCorrectOnDistanceTo(int x1, int y1, int x2, int y2)
        {
            //Act
            var location1 = Location.Create(x1, y1);
            var location2 = Location.Create(x2, y2);

            var distance = location1.Value.DistanceTo(location2.Value);

            //Assert
            distance.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ReturnErrorWhenParamsAreNotCorrectOnDistanceTo()
        {
            //Act
            var location = Location.Create(5, 5);

            var distance = location.Value.DistanceTo(null);

            //Assert
            distance.IsSuccess.Should().BeFalse();
            distance.Error.Should().NotBeNull();
        }

    }
}
