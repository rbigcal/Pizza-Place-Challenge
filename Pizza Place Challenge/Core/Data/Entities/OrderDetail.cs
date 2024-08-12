using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pizza_Place_Challenge.Core.Data.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza_Place_Challenge.Core.Data.Entities
{
    public class OrderDetail : EntityBase
    {
        #region . ASSOCIATION FIELDS                    .

        public string ID_Order { get; set; }
        public string ID_Pizza { get; set; }

        #endregion
        #region . PROPERTY FIELDS FOR SPECIFIC CLASS    .

        public int Quantity { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV    .
        // for data purposes only
        [NotMapped]
        [JsonIgnore]
        public string OrderDetailsID_FROMCSV { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string OrderID_FROMCSV { get; set; }

        [NotMapped]
        [JsonIgnore]
        public string PizzaID_FROMCSV { get; set; }
        #endregion
    }

    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetByOrderId(string orderid);
        Task<IEnumerable<OrderDetail>> GetByPizzaId(string pizzaid);
    }

    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<OrderDetail>> GetByOrderId(string orderid)
        {
            return await _dbSet.Where(orderdetail => orderdetail.ID_Order == orderid).ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByPizzaId(string pizzaid)
        {
            return await _dbSet.Where(orderdetail => orderdetail.ID_Pizza == pizzaid).ToListAsync();
        }
    }
}
