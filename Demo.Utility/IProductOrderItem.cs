using Demo.Models;

namespace Demo.Utility
{
    public interface IProductOrderItem
    {
        public double TotalCost { get; }
        public int Count { get; }
        public Product Product { get; }
    }
}