using Microsoft.EntityFrameworkCore;
using Pizza_Place_Challenge.Core.Data.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza_Place_Challenge.Core.Data.Entities
{
    public class Order : EntityBase
    {
        #region . ASSOCIATION FIELDS                    .

        #endregion
        #region . PROPERTY FIELDS FOR SPECIFIC CLASS    .

        public DateTime DateTime { get; set; }

        #endregion
        #region . PROPERTY FIELDS THAT CAME FROM CSV    .
        // for data purposes only
        [NotMapped]
        public string OrderID_FROMCSV { get; set; }

        [NotMapped]
        public string Date_FROMCSV { get; set; }

        [NotMapped]
        public string Time_FROMCSV { get; set; }
        #endregion
    }

    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetBySpecificDateTime(DateTime datetime);
        Task<IEnumerable<Order>> GetByDateTimeRange(DateTime mindatetime, DateTime maxdatetime);

    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(DbContext context) : base(context) { }

        public async Task<IEnumerable<Order>> GetBySpecificDateTime(DateTime datetime)
        {
            return await _dbSet.Where(order => order.DateTime == datetime).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateTimeRange(DateTime mindatetime, DateTime maxdatetime)
        {
            return await _dbSet.Where(order => order.DateTime >= mindatetime && order.DateTime <= maxdatetime).ToListAsync();
        }

        
    }
}
