using Domain.Entities;
using Domain.Events;
using Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record CreateOrderCommand(Guid CustomerId, List<OrderItemDto> Items) : IRequest<Guid>;

    public record OrderItemDto(Guid ProductId, decimal Price, int Quantity);

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _repository;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IOrderRepository repository, IMediator mediator, ILogger<CreateOrderCommandHandler> logger)
        {
            _repository = repository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderItems = request.Items.Select(i => new OrderItem(i.ProductId, i.Price, i.Quantity)).ToList();
            var order = new Order(request.CustomerId, orderItems);

            await _repository.AddAsync(order);
            await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // Publish domain event
            await _mediator.Publish(new OrderCreatedEvent(order.Id, order.CustomerId, order.TotalAmount), cancellationToken);

            _logger.LogInformation("Order {OrderId} created for customer {CustomerId}", order.Id, order.CustomerId);
            return order.Id;
        }
    }

}
