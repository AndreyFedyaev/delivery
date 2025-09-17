using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
        {
            if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));
            if (couriers == null || couriers.Count == 0) return GeneralErrors.ValueIsRequired(nameof(order));

            Dictionary<double, int> courierStep = new Dictionary<double, int>();

            //рассчитываем количество шагов для каждого курьера
            for (int i = 0; i < couriers.Count; i++)
            {
                var canTake = couriers[i].CanTakeOrder(order);
                if (!canTake.Value) continue;

                var step = couriers[i].CalculateTimeToLocation(order.Location);
                courierStep.Add(step.Value, i);
            }

            //определяем индекс курьера в списке с наименьшим количеством шагов
            double minStep = courierStep.Keys.Min();
            int courierStepIndex = courierStep[minStep];

            // Если курьер найден - назначаем заказ на курьера
            var orderAssignToCourierResult = order.Assign(couriers[courierStepIndex]);
            if (orderAssignToCourierResult.IsFailure) return orderAssignToCourierResult.Error;

            //назначаем курьера на заказ
            var takeOrder = couriers[courierStepIndex].TakeOrder(order);
            if (takeOrder.IsFailure) return takeOrder.Error;

            return couriers[courierStepIndex];
        }
    }
}
