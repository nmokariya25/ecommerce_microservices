using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService
{
    public class Ecom_OrderDBContext : DbContext
    {
        public Ecom_OrderDBContext(DbContextOptions<Ecom_OrderDBContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

    }
}
