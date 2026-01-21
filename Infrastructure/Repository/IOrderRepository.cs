using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public interface IOrderRepository
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
        Task AddAsync(Order order, CancellationToken ct = default);
        void Update(Order order);
        void Remove(Order order);
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
    public record OrderDto(Guid Id, Guid CustomerId, List<OrderItemDto> Items, decimal TotalAmount, OrderStatus Status);
    public record OrderSummaryDto(Guid Id, decimal TotalAmount, OrderStatus Status); 
    public record OrderItemDto(Guid ProductId, decimal Price, int Quantity);
}
