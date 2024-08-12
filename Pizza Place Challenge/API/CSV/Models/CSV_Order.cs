using CsvHelper.Configuration.Attributes;

namespace Pizza_Place_Challenge.API.CSV.Models {
    public class CSV_Order {
        [Name("order_id")]
        public string OrderId { get; set; }

        [Name("date")]
        public string Date { get; set; }

        [Name("time")]
        public string Time { get; set; }
    }
}
