using Microsoft.EntityFrameworkCore;
using Pizza_Place_Challenge.Core.Data.Entities;

namespace Pizza_Place_Challenge.Core.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<PizzaType> PizzaTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}