using CsvHelper.Configuration.Attributes;

namespace Pizza_Place_Challenge.API.CSV.Models {
    public class CSV_OrderDetails {

        [Name("order_details_id")]
        public string OrderDetailsId { get; set; }

        [Name("order_id")]
        public string OrderId { get; set; }

        [Name("pizza_id")]
        public string PizzaId { get; set; }

        [Name("quantity")]
        public string Quantity { get; set; }
    }
}
