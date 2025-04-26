using GestaoDesPedidosApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDesPedidosApi.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
