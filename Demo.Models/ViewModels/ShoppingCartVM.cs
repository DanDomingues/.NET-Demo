using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ProductList { get; set; }

        public double OrderTotal
        {
            get => ProductList.Select(p => p.TotalCost).Sum();
        }
    }
}
