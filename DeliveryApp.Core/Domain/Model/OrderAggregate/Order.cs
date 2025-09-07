using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate
{
    /// <summary>
    ///     Заказ
    /// </summary>
    class Order : Aggregate<Guid>
    {
        /// <summary>
        ///     Ctr
        /// </summary>
        private Order()
        {
        }

        /// <summary>
        ///     Ctr
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="location">Местоположение куда нужно доставить заказ</param>
        /// <param name="volume">Объём заказа</param>
        private Order(Guid orderId, Location location, int volume) : this()
        {
            Id = orderId;
            Location = location;
            Volume = volume;

            Status = OrderStatus.Created;
        }
        /// <summary>
        ///     Местоположение куда нужно доставить заказ
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        ///     Объём заказа
        /// </summary>
        public int Volume { get; private set; }

        /// <summary>
        ///     Статус заказа
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        ///     Идентификатор назначенного курьера
        /// </summary>
        public Guid? CourierId { get; private set; }

        /// <summary>
        ///     Factory Method
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <param name="location">Местоположение куда нужно доставить заказ</param>
        /// <param name="volume">Объём заказа</param>
        /// 
        /// <returns>Результат</returns>
        public static Result<Order, Error> Create(Guid orderId, Location location, int volume)
        {
            if (orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
            if (volume <= 0) return GeneralErrors.ValueIsInvalid(nameof(volume));

            return new Order(orderId, location, volume);
        }

        /// <summary>
        ///     Назначение заказа на курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>результат</returns>
        public UnitResult<Error> Assign(Courier courier)
        {
            if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));
            if (Status == OrderStatus.Created) return new Error($"{nameof(courier).ToLowerInvariant()}", "Заказ уже назначен на курьера!");
            CourierId = courier.Id;
            Status = OrderStatus.Assigned;

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Завершение заказа
        /// </summary>
        /// <returns>результат</returns>
        public UnitResult<Error> Complete()
        {
            if (Status != OrderStatus.Assigned) return new Error($"{nameof(Order).ToLowerInvariant()}", "Нельзя завершить заказ, который небыл назначен на курьера!");
            if (CourierId == null) return new Error($"{nameof(Order).ToLowerInvariant()}", "Нельзя заавершить заказ, который небыл назначен на курьера!");

            Status = OrderStatus.Completed;
            CourierId = null;

            return UnitResult.Success<Error>();
        }


    }
}
