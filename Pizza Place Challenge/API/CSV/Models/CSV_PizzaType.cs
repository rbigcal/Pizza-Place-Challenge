using CsvHelper.Configuration.Attributes;
using ServiceStack;

namespace Pizza_Place_Challenge.API.CSV.Models {
    public class CSV_PizzaType {
        [Name("pizza_type_id")]
        public string PizzaTypeId { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("category")]
        public string Category { get; set; }

        [Name("ingredients")]
        public string Ingredients { get; set; }
    }
}
