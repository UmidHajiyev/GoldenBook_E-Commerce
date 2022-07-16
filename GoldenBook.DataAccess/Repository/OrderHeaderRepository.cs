using GoldenBook.DataAccess.Repository.IRepository;
using GoldenBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var OrderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (OrderFromDb != null)
            {
                OrderFromDb.OrderStatus = orderStatus;
                if (paymentStatus!=null)
                {
                    OrderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}
