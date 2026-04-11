using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northstar.Models
{
    public class ProductImage : ModelBase, IOrderableModel
    {
        [Required] public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int ProductId { get; set; }
    }
}
