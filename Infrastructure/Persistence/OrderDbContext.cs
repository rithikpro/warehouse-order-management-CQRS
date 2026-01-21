using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class OrderDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(o => o.Id);
                b.OwnsMany(o => o.Items, i =>
                {
                    i.WithOwner().HasForeignKey("OrderId");
                    i.Property<int>("Id").ValueGeneratedOnAdd();
                    i.HasKey("Id");
                });
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await base.SaveChangesAsync(ct);
        }
    }

}
