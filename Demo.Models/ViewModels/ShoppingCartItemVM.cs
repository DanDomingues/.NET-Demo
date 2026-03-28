namespace Demo.Models.ViewModels
{
    public class ShoppingCartItemVM
    {
        public Product Product { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        public int ProductId { get; set; }
        public int Count { get; set; } = 1;
    }
}