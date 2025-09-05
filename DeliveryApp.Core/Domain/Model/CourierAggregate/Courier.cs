using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    ///     Курьер
    /// </summary>
    class Courier : Aggregate<Guid>
    {
        /// <summary>
        ///     Константы 
        /// </summary>
        private const int storagePlaceVolume = 10;

        /// <summary>
        ///     Ctr
        /// </summary>
        private Courier()
        {
        }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="buyerId">Идентификатор покупателя</param>
        private Courier(string name, int speed, Location location, StoragePlace storagePlace) : this()
        {
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
        public  List<StoragePlace> StoragePlaces { get; private set; }

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
        public UnitResult<Error> AddStoragePlace(string mame, int volume)
        {
            if (String.IsNullOrEmpty(mame)) return GeneralErrors.ValueIsRequired(nameof(mame));
            if (volume <= 0) return GeneralErrors.ValueIsInvalid(nameof(volume));

            var storagePlace = StoragePlace.Create(mame, volume);
            if (storagePlace.IsFailure) return storagePlace.Error;
            StoragePlaces.Add(storagePlace.Value);

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Проверка на возможность взять заказ
        /// </summary>
        /// <param name="volume">Объём</param>
        /// <returns>статус</returns>
        public Result<bool, Error> CanTakeorder(Order order)
        {
            foreach (var storagePlace in StoragePlaces)
            {
                if (storagePlace.CanStore(order.Volume).IsSuccess)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
