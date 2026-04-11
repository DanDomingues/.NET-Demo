using Demo.Models;

namespace Demo.Models
{
    public interface IProductOrderItem
    {
        public double TotalCost { get; }
        public int Count { get; }
        public Product Product { get; }
    }
}