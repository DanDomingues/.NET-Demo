namespace Demo.Models.ViewModels
{
    public class ShoppingCartItemVM
    {
        public Product Product { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}