using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context) => _context = context;

        // Expose DbContext as UnitOfWork
        public IUnitOfWork UnitOfWork => _context;

        public async Task<Order> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }


        public async Task<List<OrderDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderDto(o.Id, o.CustomerId, null!, o.TotalAmount, o.Status))
                .ToListAsync();
        }



        public async Task AddAsync(Order order, CancellationToken ct = default)
        {
            await _context.Orders.AddAsync(order, ct);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }

        Task<List<Order>> IOrderRepository.GetByCustomerIdAsync(Guid customerId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }


}
