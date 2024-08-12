using CsvHelper.Configuration.Attributes;
using ServiceStack;

namespace Pizza_Place_Challenge.API.CSV.Models {
    public class CSV_Pizza {
        [Name("pizza_id")]
        public string PizzaId { get; set; }

        [Name("pizza_type_id")]
        public string PizzaTypeId { get; set; }

        [Name("size")]
        public string Size { get; set; }

        [Name("price")]
        public string Price { get; set; }
    }
}
