using Domain.Entities;
using Infrastructure.Repository;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;

    public record OrderDto(Guid Id, Guid CustomerId, List<OrderItemDto> Items, decimal TotalAmount, OrderStatus Status);

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IOrderRepository _readRepository;

        public GetOrderByIdQueryHandler(IOrderRepository readRepository) => _readRepository = readRepository;

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _readRepository.GetByIdAsync(request.OrderId, cancellationToken);
            return order?.Adapt<OrderDto>();  
        }
    }

}
