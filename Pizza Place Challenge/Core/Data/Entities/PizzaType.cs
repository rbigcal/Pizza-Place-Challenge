using Pizza_Place_Challenge.Core.Data.Base;
using Pizza_Place_Challenge.Core.Enumerations;

namespace Pizza_Place_Challenge.Core.Data.Entities
{
    public class PizzaTypes : EntityBase
    {
        #region . ASSOCIATION FIELDS .

        #endregion

        #region . PROPERTY FIELDS FOR SPECIFIC CLASS .

        public string Name { get; set; }
        public PizzaCategories_Enumeration Category { get; set; }
        public string Ingredients { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV .
        // for data purposes only
        public string PizzaType_FromCSV { get; set; }
        #endregion
    }
}
