using Microsoft.EntityFrameworkCore;
using Pizza_Place_Challenge.Core.Data.Entities;

namespace Pizza_Place_Challenge.Core.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        
        public DbSet<Pizza> Pizza { get; set; }
        public DbSet<PizzaType> PizzaType { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
    }
}