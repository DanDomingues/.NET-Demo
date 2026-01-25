using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void UpdateOrderStatus(int id, string status);
        void UpdatePaymentStatus(int id, string status);
        void UpdatePaymentID(int id, string sessionId, string intentId);
    }
}
