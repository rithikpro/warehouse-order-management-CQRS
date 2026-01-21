using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid CustomerId { get; private set; }
        public List<OrderItem> Items { get; private set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        private Order() { }  
        public Order(Guid customerId, List<OrderItem> items)
        {
            CustomerId = customerId;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public void Confirm() => Status = OrderStatus.Confirmed;
        public void Ship() => Status = OrderStatus.Shipped;
    }

    public enum OrderStatus { Pending, Confirmed, Shipped, Cancelled }
    public record OrderItem(Guid ProductId, decimal Price, int Quantity);

}
