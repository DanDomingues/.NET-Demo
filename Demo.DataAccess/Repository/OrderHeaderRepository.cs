using Demo.DataAccess.Data;
using Demo.DataAccess.Repository.IRepository;
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
        protected void IfIdValid(Action<OrderHeader> action, int id)
        {
            var obj = GetById(id);
            if(obj == null)
            {
                return;
            }
            action.Invoke(obj);
        }

        protected void IfIdAndArgsValid(Action<OrderHeader> action, int id, params string[] args)
        {
            if(args?.Any(a => string.IsNullOrEmpty(a)) != false)
            {
                return;
            }
            IfIdValid(action, id);
        }

        protected void IfArgsValid(Action<OrderHeader> action, int id, params string[] args)
        {
            if (args?.Any(a => string.IsNullOrEmpty(a)) != false)
            {
                return;
            }
            action?.Invoke(GetById(id));
        }

        public void UpdateOrderStatus(int id, string status)
        {
            IfIdAndArgsValid(
                obj => obj.OrderStatus = status,
                id,
                status);
        }

        public void UpdatePaymentStatus(int id, string status)
        {
            IfIdAndArgsValid(
                obj => obj.PaymentStatus = status,
                id,
                status);
        }

        public void UpdateSessionID(int objId, string id)
        {
            IfIdAndArgsValid(
                obj => obj.SessionId = id,
                objId,
                id);
        }

        public void UpdatePaymentID(int objId, string id)
        {
            IfIdAndArgsValid(
                obj => obj.PaymentIntentId = id,
                objId,
                id);
        }
    }
}
