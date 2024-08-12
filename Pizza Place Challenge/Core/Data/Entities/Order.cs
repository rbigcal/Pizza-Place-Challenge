using Pizza_Place_Challenge.Core.Data.Base;
using Pizza_Place_Challenge.Core.Enumerations;

namespace Pizza_Place_Challenge.Core.Data.Entities
{
    public class Order : EntityBase
    {
        #region . ASSOCIATION FIELDS .

        #endregion

        #region . PROPERTY FIELDS FOR SPECIFIC CLASS .

        public DateTime DateTime { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV .
        // for data purposes only
        public string OrderID_FROMCSV { get; set; }
        public string Date_FROMCSV { get; set; }
        public string Time_FROMCSV { get; set; }
        #endregion
    }
}
