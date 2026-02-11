using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader? Header { get; set; }
        public IEnumerable<OrderItemDetails>? Details { get; set; }
    }
}
