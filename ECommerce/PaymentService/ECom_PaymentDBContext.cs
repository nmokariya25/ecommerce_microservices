using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService
{
    public class ECom_PaymentDBContext : DbContext
    {
        public ECom_PaymentDBContext(DbContextOptions<ECom_PaymentDBContext> options) : base(options) { }

        public DbSet<Payment> Payments { get; set; }
    }
}
