﻿using Microsoft.EntityFrameworkCore;
using Pizza_Place_Challenge.Core.Data.Base;
using Pizza_Place_Challenge.Core.Enumerations;
using System.Text.Json.Serialization;

namespace Pizza_Place_Challenge.Core.Data.Entities {
    public class Pizza : EntityBase
    {

        #region . ASSOCIATION FIELDS                    .

        // this property is a foreign key relation to Pizza Type
        [JsonPropertyName("id_pizza_type")]
        public string ID_PizzaType { get; set; }

        #endregion
        #region . PROPERTY FIELDS FOR SPECIFIC CLASS    .

        [JsonPropertyName("pizza_id")]
        public string PizzaId { get; set; } // much better to leave this here pizza_id is like a combination of the pizza type and size

        [JsonPropertyName("size")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PizzaSizes_Enumeration Size { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        #endregion

    }

    public interface IPizzaRepository : IRepository<Pizza>
    {
        // for specific methods or queries
        Task<List<Pizza>> GetByPizzaType(string pizzatypeid);
        Task<List<Pizza>> GetPizzaByPriceRange(double minprice, double maxprice);
    }

    public class PizzaRepository : Repository<Pizza>, IPizzaRepository
    {
        public PizzaRepository(DbContext context) : base(context) { }

        public async Task<List<Pizza>> GetByPizzaType(string pizzatypeid)
        {
            return await _dbSet.Where(pizza => pizza.ID_PizzaType == pizzatypeid).ToListAsync();
        }

        public async Task<List<Pizza>> GetPizzaByPriceRange(double minprice, double maxprice)
        {
            return await _dbSet.Where(pizza => pizza.Price >= minprice && pizza.Price <= maxprice).ToListAsync();
        }
    }
}
