using Northstar.Models;

namespace Northstar.Models
{
    public interface IProductOrderItem
    {
        public double TotalCost { get; }
        public int Count { get; }
        public Product Product { get; }
    }
}