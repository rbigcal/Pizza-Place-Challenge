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

        [JsonProperty(PropertyName = "id_order")]
        public string ID_Order { get; set; }

        [JsonProperty(PropertyName = "id_pizza")]
        public string ID_Pizza { get; set; }

        #endregion
        #region . PROPERTY FIELDS FOR SPECIFIC CLASS    .

        [JsonProperty(PropertyName = "status")]
        public int Quantity { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV    .

        #endregion
    }

    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<List<OrderDetail>> GetByOrderId(string orderid);
        Task<List<OrderDetail>> GetByPizzaId(string pizzaid);
    }

    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DbContext context) : base(context) { }

        public async Task<List<OrderDetail>> GetByOrderId(string orderid)
        {
            return await _dbSet.Where(orderdetail => orderdetail.ID_Order == orderid).ToListAsync();
        }

        public async Task<List<OrderDetail>> GetByPizzaId(string pizzaid)
        {
            return await _dbSet.Where(orderdetail => orderdetail.ID_Pizza == pizzaid).ToListAsync();
        }
    }
}
