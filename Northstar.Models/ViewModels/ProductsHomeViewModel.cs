namespace Northstar.Models.ViewModels
{
    public class ProductsHomeViewModel
    {
        public IEnumerable<Product> Products { get; set; } = null!;
        public Dictionary<int, Category> Categories { get; set; } = null!;
        public Dictionary<int, IEnumerable<Product>> CategoryProducts { get; set; } = null!;
    }
}