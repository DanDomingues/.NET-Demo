using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Category : ModelBase, INamedModel, IOrderableModel
    {
        [Required, MaxLength(20), DisplayName("Category Name")]
        public string? Name { get; set; } = null!;
        
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

        public string Description
        {
            //TODO-1: Update this as part of the model and add texts to model seeding statement
            get
            {
                var categoryDescriptions = new Dictionary<string, string>
                {
                    ["Workspace"] = "Desk pieces and focused essentials for cleaner workdays.",
                    ["Tech Accessories"] = "Compact gear and accessories that fit modern routines.",
                    ["Organization"] = "Storage, order, and small systems that reduce clutter.",
                    ["Travel"] = "Portable picks for commutes, weekends, and on-the-go carry.",
                    ["Home Essentials"] = "Reliable products for daily maintenance and easy living."
                };

                return categoryDescriptions.TryGetValue(Name ?? string.Empty, out var description) ? description : string.Empty;
            }
        }
    }
}