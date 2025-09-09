using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    /// <summary>
    ///     Место хранения курьера
    /// </summary>
    public class StoragePlace : Entity<Guid>
    {
        /// <summary>
        ///     Ctr
        /// </summary>
        private StoragePlace()
        {
        }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="good">Товар</param>
        /// <param name="quantity">Количество</param>
        private StoragePlace(string name, int volume) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            TotalVolume = volume;
        }

        /// <summary>
        ///     Название места хранения (рюкзак, багажник и т.п.)
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Допустимый объем места хранения
        /// </summary>
        public int TotalVolume { get; private set; }

        /// <summary>
        ///     Идентификатор заказа, который хранится в месте хранения
        /// </summary>
        public Guid? OrderId { get; private set; }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="name">Название места хранения</param>
        /// <param name="volume">Объём</param>
        /// <returns>Результат</returns>
        public static Result<StoragePlace, Error> Create(string name, int volume)
        {
            if (String.IsNullOrEmpty(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (volume <= 0) return GeneralErrors.ValueIsInvalid(nameof(volume));

            return new StoragePlace(name, volume);
        }

        /// <summary>
        ///     Проверка на возможность добавления товара указанного объёма
        /// </summary>
        /// <param name="volume">Объём</param>
        /// <returns>статус</returns>
        public Result<bool, Error> CanStore(int volume)
        {
            if (volume <= 0 || volume > TotalVolume) return GeneralErrors.ValueIsInvalid(nameof(volume));
            return IsOccupied();
        }

        /// <summary>
        ///     Добавление товара
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="volume">Объём</param>
        /// <returns>результат</returns>
        public UnitResult<Error> Store(Guid orderId, int volume)
        {
            if (volume <= 0 || volume > TotalVolume) return GeneralErrors.ValueIsInvalid(nameof(volume));

            if (!IsOccupied()) return GeneralErrors.ValueIsInvalid(nameof(orderId));

            OrderId = orderId;

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Удаление товара
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>результат</returns>
        public UnitResult<Error> Clear(Guid orderId)
        {
            if (orderId != OrderId) return GeneralErrors.ValueIsInvalid(nameof(orderId));
            OrderId = null;
            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Проверка занятости места хранения
        /// </summary>
        /// <returns>статус</returns>
        private bool IsOccupied()
        {
            if (OrderId != null) return false;
            return true;
        }
    }
}
