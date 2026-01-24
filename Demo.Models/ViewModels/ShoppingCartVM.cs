using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCartItem> ProductList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
