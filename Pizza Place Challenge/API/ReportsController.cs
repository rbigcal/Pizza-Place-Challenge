using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pizza_Place_Challenge.API.Base.Models;
using Pizza_Place_Challenge.API.CSV.Models;
using Pizza_Place_Challenge.Core.Data;
using Pizza_Place_Challenge.Core.Data.Entities;
using System.Globalization;
using System.Net;
using static Pizza_Place_Challenge.API.PizzaTypesController;

namespace Pizza_Place_Challenge.API {
    [Route("api/reports")]
    [ApiController, AllowAnonymous]
    [ApiExplorerSettings(GroupName = "Reports")]
    public class ReportsController : ControllerBase {
        #region . Setup                .
        private DataContext _context { get; set; }

        public ReportsController(DataContext context) {
            _context = context;
        }
        #endregion
        #region . Locals               .

        #endregion

        #region . API Endpoint Models  .

        public class OrderDetailItemModel {
            public string PizzaCode { get; set; }
            public string PizzaName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
            public double Total { get; set; }
        }

        public class OrderDetailModel : ApiControllerModel {
            public string OrderId { get; set; }
            public string DateOrdered { get; set; }
            public List<OrderDetailItemModel> Items { get; set; }
            public double Total { get; set; }
        }

        public class TotalSalesByPizzaOrderItemModel {
            public string DateOrdered { get; set; }
            public int Quantity { get; set; }
            public double Total { get; set; }
        }

        public class TotalSalesByPizza : ApiControllerModel {
            public Pizza Pizza { get; set; }
            public PizzaType PizzaType { get; set; }
            public List<TotalSalesByPizzaOrderItemModel> Items { get; set; }
            public double Total { get; set; }

        }

        #endregion
        #region . API Endpoints        .

        [HttpGet, Route("query/order-receipt"), AllowAnonymous]
        public async Task<OrderDetailModel> GetOrderReceipt(string orderid) {
            OrderDetailModel result = new();

            try {
                
                PizzaTypeRepository pizzatype_repository = new PizzaTypeRepository(_context);
                PizzaRepository pizza_repository = new PizzaRepository(_context);
                OrderRepository order_repository = new OrderRepository(_context);
                OrderDetailRepository orderdetail_repository = new OrderDetailRepository(_context);

                Order order = await order_repository.GetByIdAsync(orderid);

                if(order == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order not found");
                    return result;
                }

                List<OrderDetail> orderdetails = await orderdetail_repository.GetByOrderId(orderid);

                if(!orderdetails.Any()) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order Details Not Found");
                    return result;
                }

                // look up table
                List<Pizza> pizza_list = await pizza_repository.GetAllAsync();
                List<PizzaType> pizzatype_list = await pizzatype_repository.GetAllAsync();

                List<OrderDetailItemModel> orderdetail_items = new List<OrderDetailItemModel>();

                double running_total = 0;

                foreach(OrderDetail orderdetail in orderdetails) {

                    Pizza pizza = pizza_list.FirstOrDefault(i => i.Id == orderdetail.ID_Pizza);
                    PizzaType pizzatype = null;
                    if (pizza != null) {
                        pizzatype = pizzatype_list.FirstOrDefault(i => i.Id == pizza.ID_PizzaType);
                    }

                    if(pizza != null && pizzatype != null) {

                        OrderDetailItemModel orderdetail_item = new OrderDetailItemModel() {
                            PizzaCode = pizza.PizzaId,
                            PizzaName = pizzatype.Name,
                            Price = pizza.Price,
                            Quantity = orderdetail.Quantity,
                            Total = pizza.Price * orderdetail.Quantity
                        };

                        running_total = running_total + orderdetail_item.Total;
                        orderdetail_items.Add(orderdetail_item);
                    }
                }

                
                result.SetStatus(HttpStatusCode.OK, string.Empty);
                result.DateOrdered = order.DateTime.ToString("dddd, dd MMMM yyyy hh:mm tt");
                result.OrderId = order.Id;
                result.Items = orderdetail_items;
                result.Total = running_total;

            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet, Route("query/total-sales-by-pizza"), AllowAnonymous]
        public async Task<TotalSalesByPizza> GetTotalSalesByPizza(string pizzaid) {
            TotalSalesByPizza result = new();

            try {

                PizzaTypeRepository pizzatype_repository = new PizzaTypeRepository(_context);
                PizzaRepository pizza_repository = new PizzaRepository(_context);
                OrderRepository order_repository = new OrderRepository(_context);
                OrderDetailRepository orderdetail_repository = new OrderDetailRepository(_context);

                Pizza pizza = await pizza_repository.GetByIdAsync(pizzaid);

                if (pizza == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza Information Not Found");
                    return result;
                }

                PizzaType pizzatype = await pizzatype_repository.GetByIdAsync(pizza.ID_PizzaType);

                if (pizzatype == null) {
                    result.SetStatus(HttpStatusCode.NotFound, "Pizza Type Information Not Found");
                    return result;
                }

                List<OrderDetail> orderdetails = await orderdetail_repository.GetByPizzaId(pizzaid);

                if (!orderdetails.Any()) {
                    result.SetStatus(HttpStatusCode.NotFound, "Order Details Not Found");
                    return result;
                }

                List<Order> orders = await order_repository.GetAllAsync();
                List<TotalSalesByPizzaOrderItemModel> items = new List<TotalSalesByPizzaOrderItemModel>();

                double runningtotal = 0;

                foreach(OrderDetail orderdetail in orderdetails) {

                    Order order = orders.FirstOrDefault(i => i.Id == orderdetail.ID_Order);

                    if(order != null) {
                        TotalSalesByPizzaOrderItemModel new_pizzaorderitem = new TotalSalesByPizzaOrderItemModel() {
                            DateOrdered = order.DateTime.ToString("dddd, dd MMMM yyyy hh:mm tt"),
                            Quantity = orderdetail.Quantity,
                            Total = orderdetail.Quantity * pizza.Price
                        };
                        runningtotal = runningtotal + new_pizzaorderitem.Total;
                        items.Add(new_pizzaorderitem);
                    }
                }

                result.SetStatus(HttpStatusCode.OK, string.Empty);
                result.Pizza = pizza;
                result.PizzaType = pizzatype;
                result.Items = items;
                result.Total = runningtotal;

            } catch (Exception ex) {
                result.SetStatus(HttpStatusCode.InternalServerError, ex.Message);
            }

            return result;
        }

        #endregion

        #region . Helper Methods       .

        #endregion
    }
}

