

using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pizza_Place_Challenge.API.Base.Models;
using Pizza_Place_Challenge.API.CSV.Models;
using Pizza_Place_Challenge.Core.Data;
using Pizza_Place_Challenge.Core.Data.Entities;
using System.Globalization;
using System.Net;

namespace Pizza_Place_Challenge.API {
    [Route("api/order")]
    [ApiController, AllowAnonymous]
    public class OrderController : ControllerBase {
        #region . Setup                .
        private DataContext _context { get; set; }

        public OrderController(DataContext context) {
            _context = context;
        }
        #endregion
        #region . Locals               .

        #endregion

        #region . API Endpoint Models  .

        public class AllOrderModel : ApiControllerModel {
            [JsonProperty(PropertyName = "orders")]
            public List<Order> Orders { get; set; } = new();
        }

        public class ByOrderModel : ApiControllerModel {
            [JsonProperty(PropertyName = "order")]
            public Order Order { get; set; }
        }

        public class NewEditOrderModel : ApiControllerModel {
            [JsonProperty(PropertyName = "order")]
            public Order Order { get; set; }
        }

        public class ORDER_DTO_OrderInfo {
            [JsonProperty(PropertyName = "pizzaid")]
            public string PizzaId { get; set; }

            [JsonProperty(PropertyName = "quantity")]
            public int Quantity { get; set; }
        }

        #endregion
        #region . API Endpoints        .

        [HttpGet, Route("query/all"), AllowAnonymous]
        public async Task<AllOrderModel> GetAllOrdersAsync(int skip = 0, int shownumberofrecords = 10)
        {
            AllOrderModel result = new();

            try
            {
                OrderRepository repository = new OrderRepository(_context);

                List<Order> orders = await repository.GetAllAsync();

                result.Orders = orders.OrderBy(o => o.DateTime).Skip(skip).Take(shownumberofrecords).ToList();

            }
            catch (Exception ex) 
            { 
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/by-id"), AllowAnonymous]
        public async Task<ByOrderModel> ByOrderIdAsync(
            string id
        )
        {
            ByOrderModel result = new();

            try {
                OrderRepository repository = new OrderRepository(_context);
                Order order = await repository.GetByIdAsync(id);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                result.Order = order;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/new"), AllowAnonymous]
        public async Task<NewEditOrderModel> NewOrderAsync()
        {
            NewEditOrderModel result = new();

            try
            {

                OrderRepository repository = new OrderRepository(_context);

                Order order = new Order() {
                    DateTime = DateTime.Now
                };

                await repository.AddAsync(order);
                result.Order = order;
            }
            catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/new-order-with-details"), AllowAnonymous]
        public async Task<NewEditOrderModel> NewOrderWithDetailsAsync(List<ORDER_DTO_OrderInfo> input_params) {
            NewEditOrderModel result = new();

            try {

                OrderRepository order_repository = new OrderRepository(_context);
                OrderDetailRepository orderdetail_repository = new OrderDetailRepository(_context);

                Order order = new Order() {
                    DateTime = DateTime.Now
                };

                await order_repository.AddAsync(order);

                List<OrderDetail> orderdetails = new List<OrderDetail>();

                if (input_params.Any()) { 
                    foreach(ORDER_DTO_OrderInfo order_info in  input_params) {
                        OrderDetail new_orderdetail = new OrderDetail() {
                            ID_Order = order.Id,
                            ID_Pizza = order_info.PizzaId,
                            Quantity = order_info.Quantity
                        };

                        await orderdetail_repository.AddAsync(new_orderdetail);

                        orderdetails.Add(new_orderdetail);
                    }
                }

                result.Order = order;
            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/upload-csv"), AllowAnonymous]
        public async Task<AllOrderModel> NewOrderUploadCSVAsync(IFormFile order_csv) {
            AllOrderModel result = new();

            try {

                OrderRepository repository = new OrderRepository(_context);

                List<Order> neworders_list = new();
                List<CSV_Order> orders_fromcsvlist = new();

                if (order_csv == null || order_csv.Length == 0) {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Cannot read csv file");
                }

                if (order_csv != null) {
                    using (var stream = order_csv.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture))) {
                        orders_fromcsvlist = csv.GetRecords<CSV_Order>().ToList();
                    }
                }

                foreach (CSV_Order order_fromcsv in orders_fromcsvlist) {

                    DateTime datetimefromcsv = DateTime.UtcNow;

                    DateTime dateOnly = DateTime.Parse(order_fromcsv.Date);
                    DateTime timeOnly = DateTime.Parse(order_fromcsv.Time);

                    datetimefromcsv = dateOnly.Date.Add(timeOnly.TimeOfDay);

                    int orderid_fromcsv = -1;

                    if (int.TryParse(order_fromcsv.OrderId, out orderid_fromcsv)) {

                    } else {
                        orderid_fromcsv = -1;
                    }

                    Order new_order = new Order() {
                        DateTime = datetimefromcsv,
                        OrderId_FromCSV = orderid_fromcsv
                    };

                    neworders_list.Add(new_order);
                }
                
                await repository.AddAsync(neworders_list);

                // THIS IS JUST TO SHOW 200 RECORDS a large amount of data will cause swagger to be sluggish
                result.Orders = neworders_list.Skip(0).Take(200).ToList();
            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPatch, Route("action/edit"), AllowAnonymous]
        public async Task<NewEditOrderModel> EditOrderAsync(string id)
        {
            NewEditOrderModel result = new();

            try
            {
                OrderRepository repository = new OrderRepository(_context);

                Order order = await repository.GetByIdAsync(id);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                order.DateTime = DateTime.Now;

                await repository.UpdateAsync(order);
                result.Order = order;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpDelete, Route("action/delete"), AllowAnonymous]
        public async Task<ApiControllerModel> DeleteOrderAsync(string id)
        {
            ApiControllerModel result = new();

            try
            {
                OrderRepository repository = new OrderRepository(_context);

                Order order = await repository.GetByIdAsync(id);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                await repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }



        #endregion

        #region . Helper Methods       .

        #endregion
    }
}

