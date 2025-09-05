using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate
{
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
        /// <param name="buyerId">Идентификатор покупателя</param>
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
            if (location == null) return GeneralErrors.ValueIsRequired(nameof(location));
            if (volume <= 0) return GeneralErrors.ValueIsInvalid(nameof(volume));

            return new Order(orderId, location, volume);
        }

        /// <summary>
        ///     Назначение на курьера
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>результат</returns>
        public UnitResult<Error> Assign(Courier courier)
        {
            if (courier == null) return GeneralErrors.ValueIsRequired(nameof(courier));

            CourierId = courier.Id;
            Status = OrderStatus.Assigned;

            return UnitResult.Success<Error>();
        }

        /// <summary>
        ///     Завершение заказа
        /// </summary>
        /// <returns>результат</returns>
        public UnitResult<Error> Assign()
        {
            if (Status == OrderStatus.Assigned)
            {
                Status = OrderStatus.Completed;
                CourierId = null;
            }
            return UnitResult.Success<Error>();
        }
    }
}
