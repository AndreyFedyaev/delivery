using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http
{
    public class DeliveryController : DefaultApiController
    {
        private readonly IMediator _mediator;
        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override Task<IActionResult> CreateCourier([FromBody] NewCourier newCourier)
        {
            throw new NotImplementedException();
        }

        public override async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            var street = "Несуществующая";
            var createOrderCommand = CreateOrderCommand.Create(orderId, street,3);

            var response = await _mediator.Send(createOrderCommand.Value);
            if (response.IsSuccess) return Ok();
            return Conflict();
        }

        public override async Task<IActionResult> GetCouriers()
        {
            var getAllCouriers = new GetAllCouriersQuery();
            var response = await _mediator.Send(getAllCouriers);

            if (response == null) return NotFound();

            //маппинг
            var couriers = response.Couriers.Select(a => new Courier
            {
                Id = a.Id,
                Name = a.Name,
                Location = new Location { X = a.Location.X, Y = a.Location.Y}
            });

            return Ok(couriers);
        }

        public override async Task<IActionResult> GetOrders()
        {
            var getOrders = new GetNotCompletedOrdersQuery();
            var response = await _mediator.Send(getOrders);

            if (response == null) return NotFound();

            //маппинг
            var orders = response.Orders.Select(a => new Order
            {
                Id = a.Id,
                Location = new Location { X = a.Location.X, Y = a.Location.Y }
            });

            return Ok(orders);
        }
    }
}
