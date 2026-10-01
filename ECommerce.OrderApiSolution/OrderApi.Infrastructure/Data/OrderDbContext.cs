using Microsoft.EntityFrameworkCore;
using OrderApi.Domain.Entites;


namespace OrderApi.Infrastructure.Data
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Order>().Property(p => p.ProductId).HasColumnOrder("Product Id");
        //}

        public DbSet<Order> MSOrders { get; set; }
    }
}
