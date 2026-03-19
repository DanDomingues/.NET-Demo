using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }

        //External
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        //Keys
        public int ProductId { get; set; }
    }
}