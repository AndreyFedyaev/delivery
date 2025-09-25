using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    ///     Курьер
    /// </summary>
    public class Courier : Aggregate<Guid>
    {
        /// <summary>
        ///     Константы 
        /// </summary>
        private const int storagePlaceVolume = 10;

        /// <summary>
        ///     Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Courier()
        {
        }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="buyerId">Идентификатор покупателя</param>
        private Courier(string name, int speed, Location location, StoragePlace storagePlace) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Speed = speed;
            Location = location;
            StoragePlaces.Add(storagePlace);
        }

        /// <summary>
        ///     Имя курьера
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Скорость курьера
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        ///     Местоположение курьера
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        ///     Места хранения курьера
        /// </summary>
        public List<StoragePlace> StoragePlaces { get; private set; } = new List<StoragePlace>();

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="name">Имя курьера</param>
        /// <param name="speed">Скорость курьера</param>
        /// <param name="location">Местоположение курьера</param>
        /// <returns>Результат</returns>
        public static Result<Courier, Error> Create(string name, int speed, Location location)
        {
            if (String.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (speed <= 0) return GeneralErrors.ValueIsInvalid(nameof(speed));

            var storagePlace = StoragePlace.Create("Bag", storagePlaceVolume);
            if (storagePlace.IsFailure) return storagePlace.Error;

            return new Courier(name, speed, location, storagePlace.Value);
        }

        /// <summary>
        ///     Добавление места хранения курьеру
        /// </summary>
        /// <param name="mame">название места хранения</param>
        /// <param name="volume">Объём</param>
        /// <returns>результат</returns>
        public UnitResult<Error> AddStoragePlace(string name, int volume)
        {
            if (String.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (volume <= 0) return GeneralErrors.ValueIsInvalid(nameof(volume));

            var storagePlace = StoragePlace.Create(name, volume);
            if (storagePlace.IsFailure) return storagePlace.Error;
            StoragePlaces.Add(storagePlace.Value);

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Проверка на возможность взять заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>статус</returns>
        public Result<bool, Error> CanTakeOrder(Order order)
        {
            if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));

            foreach (var storagePlace in StoragePlaces)
            {
                if (storagePlace.CanStore(order.Volume).IsSuccess)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Взять заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>статус</returns>
        public UnitResult<Error> TakeOrder(Order order)
        {
            if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));

            foreach (var storagePlace in StoragePlaces)
            {
                if (storagePlace.CanStore(order.Volume).IsSuccess)
                {
                    var storeResult = storagePlace.Store(order.Id, order.Volume);
                    if (storeResult.IsFailure) return storeResult.Error;
                    break;
                }
            }

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Завершить заказ
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>статус</returns>
        public UnitResult<Error> ComplateOrder(Order order)
        {
            if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));

            foreach (var storagePlace in StoragePlaces)
            {
                if (storagePlace.OrderId == order.Id)
                {
                    var clearResult = storagePlace.Clear(order.Id);
                    if (clearResult.IsFailure) return clearResult.Error;
                    break;
                }
            }

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Рассчет количества шагов на путь до локации заказа
        /// </summary>
        /// <param name="location">Локация для рассчета</param>
        /// <returns>Количество шагов</returns>
        public Result<double, Error> CalculateTimeToLocation(Location location)
        {
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));

            var distanceToResult = Location.DistanceTo(location);
            if (distanceToResult.IsFailure) return distanceToResult.Error;

            var result = (double)distanceToResult.Value / Speed;
            return result;
        }

        /// <summary>
        ///     Изменить местоположение
        /// </summary>
        /// <param name="target">Целевое местоположение</param>
        /// <returns>Местоположение после сдвига</returns>
        public UnitResult<Error> Move(Location target)
        {
            if (target == null) return GeneralErrors.ValueIsRequired(nameof(target));

            var difX = target.X - Location.X;
            var difY = target.Y - Location.Y;
            var cruisingRange = Speed;

            var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
            cruisingRange -= Math.Abs(moveX);

            var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

            var locationCreateResult = Location.Create(Location.X + moveX, Location.Y + moveY);
            if (locationCreateResult.IsFailure) return locationCreateResult.Error;
            Location = locationCreateResult.Value;

            return UnitResult.Success<Error>();
        }
    }
}
