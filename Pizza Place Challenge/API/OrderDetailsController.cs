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
    [Route("api/order-details")]
    [ApiController, AllowAnonymous]
    [ApiExplorerSettings(GroupName = "Order Details")]
    public class OrderDetailsController : ControllerBase {
        #region . Setup                .
        private DataContext _context { get; set; }

        public OrderDetailsController(DataContext context) {
            _context = context;
        }
        #endregion
        #region . Locals               .

        #endregion

        #region . API Endpoint Models  .

        public class AllOrderDetailsModel : ApiControllerModel {
            [JsonProperty(PropertyName = "order-details")]
            public List<OrderDetail> OrderDetails { get; set; } = new();
        }

        public class ByOrderDetailModel : ApiControllerModel {
            [JsonProperty(PropertyName = "order-details")]
            public OrderDetail OrderDetail { get; set; }
        }

        public class NewEditOrderDetailModel : ApiControllerModel {

            [JsonProperty(PropertyName = "order")]
            public Order Order { get; set; }

            [JsonProperty(PropertyName = "order-details")]
            public List<OrderDetail> OrderDetails { get; set; }

        }

        public class ORDER_DETAIL_DTO_OrderInfo {
            [JsonProperty(PropertyName = "pizzaid")]
            public string PizzaId { get; set; }

            [JsonProperty(PropertyName = "quantity")]
            public int Quantity { get; set; }
        }

        #endregion
        #region . API Endpoints        .

        [HttpGet, Route("query/all"), AllowAnonymous]
        public async Task<AllOrderDetailsModel> GetAllOrderDetailsAsync(int skip = 0, int shownumberofrecords = 10)
        {
            AllOrderDetailsModel result = new();

            try
            {
                OrderDetailRepository repository = new OrderDetailRepository(_context);

                List<OrderDetail> orderdetails_all = await repository.GetAllAsync();

                result.OrderDetails = orderdetails_all.Skip(skip).Take(shownumberofrecords).ToList();

            }
            catch (Exception ex) 
            { 
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/all-by-orderid"), AllowAnonymous]
        public async Task<AllOrderDetailsModel> GetAllOrderDetailsByOrderAsync(string orderid,int skip = 0, int shownumberofrecords = 10) {
            AllOrderDetailsModel result = new();

            try {
                OrderDetailRepository repository = new OrderDetailRepository(_context);

                List<OrderDetail> orderdetails_all = await repository.GetByOrderId(orderid);

                result.OrderDetails = orderdetails_all.Skip(skip).Take(shownumberofrecords).ToList();

            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/all-by-pizzaid"), AllowAnonymous]
        public async Task<AllOrderDetailsModel> GetAllOrderDetailsByPizzaIdAsync(string pizzaid, int skip = 0, int shownumberofrecords = 10) {
            AllOrderDetailsModel result = new();

            try {
                OrderDetailRepository repository = new OrderDetailRepository(_context);

                List<OrderDetail> orderdetails_all = await repository.GetByPizzaId(pizzaid);

                result.OrderDetails = orderdetails_all.Skip(skip).Take(shownumberofrecords).ToList();

            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/by-id"), AllowAnonymous]
        public async Task<ByOrderDetailModel> ByOrderDetailIdAsync(
            string id
        )
        {
            ByOrderDetailModel result = new();

            try {
                OrderDetailRepository repository = new OrderDetailRepository(_context);
                OrderDetail orderdetail = await repository.GetByIdAsync(id);

                if (orderdetail == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order Detail not found");
                    return result;
                }

                result.OrderDetail = orderdetail;
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/new"), AllowAnonymous]
        public async Task<NewEditOrderDetailModel> NewOrderDetailAsync(string orderid, string pizzaid, int quantity)
        {
            NewEditOrderDetailModel result = new();

            try
            {

                OrderDetailRepository repository = new OrderDetailRepository(_context);
                OrderRepository order_repository = new OrderRepository(_context);
                PizzaRepository pizza_repository = new PizzaRepository(_context);

                Order order = await order_repository.GetByIdAsync(orderid);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                Pizza pizza = await pizza_repository.GetByIdAsync(pizzaid);

                if (pizza == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza not found");
                    return result;
                }

                OrderDetail orderdetail = new OrderDetail() {
                    ID_Order = orderid,
                    ID_Pizza = pizzaid,
                    Quantity = quantity
                };

                await repository.AddAsync(orderdetail);

                result.Order = order;
                result.OrderDetails = new();
                result.OrderDetails.Add(orderdetail);
            }
            catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/new-order-details-multiple-parameter"), AllowAnonymous]
        public async Task<NewEditOrderDetailModel> NewOrderDetailsAsync(string orderid,List<ORDER_DETAIL_DTO_OrderInfo> input_params) {
            NewEditOrderDetailModel result = new();

            try {

                OrderDetailRepository orderdetail_repository = new OrderDetailRepository(_context);
                OrderRepository order_repository = new OrderRepository(_context);
                PizzaRepository pizza_repository = new PizzaRepository(_context);

                Order order = await order_repository.GetByIdAsync(orderid);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                List<Pizza> pizza_lookuplist = await pizza_repository.GetAllAsync();

                List<OrderDetail> orderdetails = new List<OrderDetail>();

                if (input_params.Any()) { 
                    foreach(ORDER_DETAIL_DTO_OrderInfo order_info in  input_params) {

                        // check first if pizza id is existing
                        Pizza pizza = pizza_lookuplist.FirstOrDefault(p => p.Id == order_info.PizzaId);

                        if(pizza == null) {
                            result.SetStatus(HttpStatusCode.NotFound, "Pizza not found");
                            return result;
                        }
                        
                        OrderDetail new_orderdetail = new OrderDetail() {
                            ID_Order = order.Id,
                            ID_Pizza = order_info.PizzaId,
                            Quantity = order_info.Quantity
                        };

                        orderdetails.Add(new_orderdetail);
                    }

                    await orderdetail_repository.AddAsync(orderdetails);

                    result.OrderDetails = orderdetails;
                }

                result.Order = order;
            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPut, Route("action/upload-csv"), AllowAnonymous]
        public async Task<AllOrderDetailsModel> NewOrderDetailUploadCSVAsync(IFormFile orderdetails_csv) {
            AllOrderDetailsModel result = new();

            try {

                OrderDetailRepository orderdetail_repository = new OrderDetailRepository(_context);
                OrderRepository order_repository = new OrderRepository(_context);
                PizzaRepository pizza_repository = new PizzaRepository(_context);

                List<OrderDetail> neworderdetails_list = new();
                List<CSV_OrderDetails> orderdetails_fromcsvlist = new();

                if (orderdetails_csv == null || orderdetails_csv.Length == 0) {
                    result.SetStatus(HttpStatusCode.InternalServerError, "Cannot read csv file");
                }

                if (orderdetails_csv != null) {
                    using (var stream = orderdetails_csv.OpenReadStream())
                    using (var reader = new StreamReader(stream))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture))) {
                        orderdetails_fromcsvlist = csv.GetRecords<CSV_OrderDetails>().ToList();
                    }
                }

                // lookup tables
                List<Order> orders_fromdb = await order_repository.GetAllAsync();
                List<Pizza> pizza_fromdb = await pizza_repository.GetAllAsync();

                foreach (CSV_OrderDetails orderdetail_fromcsv in orderdetails_fromcsvlist) {

                    int orderid = -1;
                    int quantity = 0;
                    if(int.TryParse(orderdetail_fromcsv.OrderId, out orderid)) {

                    } else {
                        orderid = -1;
                    }

                    int.TryParse(orderdetail_fromcsv.Quantity, out quantity);

                    Order order = orders_fromdb.FirstOrDefault(i => i.OrderId_FromCSV == orderid);
                    Pizza pizza = pizza_fromdb.FirstOrDefault(i => i.PizzaId == orderdetail_fromcsv.PizzaId);

                    if (order != null && pizza != null) {

                        OrderDetail new_orderdetail = new OrderDetail() {
                            ID_Order = order.Id,
                            ID_Pizza = pizza.Id,
                            Quantity = quantity
                        };

                        neworderdetails_list.Add(new_orderdetail);
                    }
                }
                
                await orderdetail_repository.AddAsync(neworderdetails_list);

                // THIS IS JUST TO SHOW 200 RECORDS a large amount of data will cause swagger to be sluggish
                result.OrderDetails = neworderdetails_list.Skip(0).Take(200).ToList();
            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPatch, Route("action/edit"), AllowAnonymous]
        public async Task<NewEditOrderDetailModel> EditOrderDetailAsync(string id, string orderid, string pizzaid, int quantity)
        {
            NewEditOrderDetailModel result = new();

            try
            {
                OrderDetailRepository repository = new OrderDetailRepository(_context);
                OrderRepository order_repository = new OrderRepository(_context);
                PizzaRepository pizza_repository = new PizzaRepository(_context);

                OrderDetail orderdetail = await repository.GetByIdAsync(id);

                if (orderdetail == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order Detail not found");
                    return result;
                }

                Order order = await order_repository.GetByIdAsync(orderid);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                Pizza pizza = await pizza_repository.GetByIdAsync(pizzaid);

                if (pizza == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza not found");
                    return result;
                }

                orderdetail.ID_Order = orderid;
                orderdetail.ID_Pizza = pizzaid;
                orderdetail.Quantity = quantity;

                await repository.UpdateAsync(orderdetail);
                result.OrderDetails = new();
                result.OrderDetails.Add(orderdetail);
            }
            catch (Exception ex)
            {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpDelete, Route("action/delete"), AllowAnonymous]
        public async Task<ApiControllerModel> DeleteOrderDetailAsync(string id)
        {
            ApiControllerModel result = new();

            try
            {
                OrderDetailRepository repository = new OrderDetailRepository(_context);

                OrderDetail order = await repository.GetByIdAsync(id);

                if (order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order Detail not found");
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

