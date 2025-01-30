using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DemoWithRazor.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(12), DisplayName("Category Name")]
        public string Name { get; set; }
        [Range(1, 100), DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}