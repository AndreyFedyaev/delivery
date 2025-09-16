using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryTests : IAsyncLifetime
    {
        /// <summary>
        ///     Настройка Postgres из библиотеки TestContainers
        /// </summary>
        /// <remarks>По сути это Docker контейнер с Postgres</remarks>
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("courier")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();

        private ApplicationDbContext _context;

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
        public CourierRepositoryTests()
        {

        }

        /// <summary>
        ///     Инициализируем окружение
        /// </summary>
        /// <remarks>Вызывается перед каждым тестом</remarks>
        public async Task InitializeAsync()
        {
            //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
            await _postgreSqlContainer.StartAsync();

            //Накатываем миграции и справочники
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                    _postgreSqlContainer.GetConnectionString(),
                    sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); })
                .Options;
            _context = new ApplicationDbContext(contextOptions);
            _context.Database.Migrate();
        }

        /// <summary>
        ///     Уничтожаем окружение
        /// </summary>
        /// <remarks>Вызывается после каждого теста</remarks>
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public async Task CanAddCourier()
        {
            //Arrange
            var location = Location.Create(3, 3).Value;
            var courier = Courier.Create("Alex", 2, location).Value;

            //Act
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var getCourierResult = await courierRepository.GetAsync(courier.Id);
            getCourierResult.HasValue.Should().BeTrue();
            var courierFromDb = getCourierResult.Value;
            courier.Should().BeEquivalentTo(courierFromDb);
        }

        [Fact]
        public async Task CanUpdateCourier()
        {
            //Arrange
            var location = Location.Create(3, 3).Value;
            var courier = Courier.Create("Alex", 2, location).Value;

            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            //Act
            var addStoragePlaceResult = courier.AddStoragePlace("BigBox", 50);
            addStoragePlaceResult.IsSuccess.Should().BeTrue();
            courierRepository.Update(courier);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var getCourierResult = await courierRepository.GetAsync(courier.Id);
            getCourierResult.HasValue.Should().BeTrue();
            var courierFromDb = getCourierResult.Value;
            courier.Should().BeEquivalentTo(courierFromDb);
        }
    }
}
