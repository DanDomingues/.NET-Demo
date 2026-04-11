using Demo.Utility;
using Demo.DataAccess.IRepository;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DataAccess.Repository
{
    public class OrderHeaderRepository(DbSet<OrderHeader> set) : Repository<OrderHeader>(set), IOrderHeaderRepository
    {
        public void UpdateOrderStatus(int id, string status)
        {
            ValidationUtility.IfIdAndArgsValid(
                id => GetById(id, tracked: true),
                obj => obj.OrderStatus = status,
                id,
                status);
        }

        public void UpdatePaymentStatus(int id, string status)
        {
            ValidationUtility.IfIdAndArgsValid(
                id => GetById(id, tracked: true),
                obj => obj.PaymentStatus = status,
                id,
                status);
        }

        public void UpdateSessionID(int objId, string id)
        {
            ValidationUtility.IfIdAndArgsValid(
                id => GetById(id, tracked: true),
                obj => obj.SessionId = id,
                objId,
                id);
        }

        public void UpdatePaymentID(int objId, string id)
        {
            ValidationUtility.IfIdAndArgsValid(
                id => GetById(id, tracked: true),
                obj => obj.PaymentIntentId = id,
                objId,
                id);
        }
    }
}
