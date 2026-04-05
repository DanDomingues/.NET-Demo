using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Category : ModelBase, INamedModel, IOrderableModel
    {
        [Required, MaxLength(20), DisplayName("Category Name")]
        public string? Name { get; set; }
        
        [Range(1, 100), DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}