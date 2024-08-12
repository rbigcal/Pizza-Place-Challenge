using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pizza_Place_Challenge.Core.Data.Base;
using Pizza_Place_Challenge.Core.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza_Place_Challenge.Core.Data.Entities
{
    public class PizzaType : EntityBase
    {
        #region . ASSOCIATION FIELDS                    .

        #endregion
        #region . PROPERTY FIELDS FOR SPECIFIC CLASS    .

        public string Name { get; set; }
        public PizzaCategories_Enumeration Category { get; set; }
        public string Ingredients { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV    .
        // for data purposes only
        [NotMapped]
        [JsonIgnore]
        public string PizzaType_FromCSV { get; set; }
        #endregion
    }

    public interface IPizzaTypeRepository : IRepository<PizzaType>
    {
        // for specific methods or queries
        Task<PizzaType> GetPizzaTypeByName(string pizzatypename);
        Task<IEnumerable<PizzaType>> GetPizzaTypeByNameLike(string pizzatypename);
        Task<IEnumerable<PizzaType>> GetPizzaTypeByCategory(PizzaCategories_Enumeration category);
    }

    public class PizzaTypeRepository : Repository<PizzaType>, IPizzaTypeRepository
    {
        public PizzaTypeRepository(DbContext context) : base(context) { }

        public async Task<PizzaType> GetPizzaTypeByName(string pizzatypename)
        {
            // from here we have two options be strict with the case of the letters
            // which will use this code 
            // await _dbSet.FirstOrDefaultAsync(pizzatype => pizzatype.Name.Equals(pizzatypename));
            // or this one which ignores case sensitivity
            // await _dbSet.FirstOrDefaultAsync(pizzatype => pizzatype.Name.ToLower().Equals(pizzatypename.ToLower()));

            return await _dbSet.FirstOrDefaultAsync(pizzatype => pizzatype.Name.ToLower().Equals(pizzatypename.ToLower()));
        }

        public async Task<IEnumerable<PizzaType>> GetPizzaTypeByNameLike(string pizzatypename)
        {
            // from here we have two options be strict with the case of the letters
            // which will use this code 
            // await _dbSet.Where(pizzatype => pizzatype.Name.Contains(pizzatypename)).ToListAsync();
            // or this one which ignores case sensitivity
            // await _dbSet.Where(pizzatype => pizzatype.Name.ToLower().Contains(pizzatypename.ToLower())).ToListAsync();

            return await _dbSet.Where(pizzatype => pizzatype.Name.ToLower().Contains(pizzatypename.ToLower())).ToListAsync();
        }

        public async Task<IEnumerable<PizzaType>> GetPizzaTypeByCategory(PizzaCategories_Enumeration category)
        {
            return await _dbSet.Where(pizzatype => pizzatype.Category == category).ToListAsync();
        }
    }
}
