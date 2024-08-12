using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pizza_Place_Challenge.Core.Data.Base;
using Pizza_Place_Challenge.Core.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza_Place_Challenge.Core.Data.Entities
{
    public class Pizza : EntityBase
    {

        #region . ASSOCIATION FIELDS                    .

        // this property is a foreign key relation to Pizza Type
        public string ID_PizzaType { get; set; }

        #endregion
        #region . PROPERTY FIELDS FOR SPECIFIC CLASS    .

        public PizzaSizes_Enumeration Size { get; set; }
        public float Price { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV    .
        // for data purposes only
        [NotMapped]
        [JsonIgnore]
        public string PizzaID_FromCSV { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string PizzaType_FromCSV { get; set; }
        #endregion

    }

    public interface IPizzaRepository : IRepository<Pizza>
    {
        // for specific methods or queries
        Task<IEnumerable<Pizza>> GetByPizzaType(string pizzatypeid);
        Task<IEnumerable<Pizza>> GetPizzaByPriceRange(float minprice, float maxprice);
    }

    public class PizzaRepository : Repository<Pizza>, IPizzaRepository
    {
        public PizzaRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<Pizza>> GetByPizzaType(string pizzatypeid)
        {
            return await _dbSet.Where(pizza => pizza.ID_PizzaType == pizzatypeid).ToListAsync();
        }

        public async Task<IEnumerable<Pizza>> GetPizzaByPriceRange(float minprice, float maxprice)
        {
            return await _dbSet.Where(pizza => pizza.Price >= minprice && pizza.Price <= maxprice).ToListAsync();
        }
    }
}
