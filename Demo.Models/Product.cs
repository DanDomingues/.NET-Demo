using Demo.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models
{
    public class Product : NamedModel
    {
        [Required, Display(Name = "SKU")] public string ISBN { get; set; } = null!;
        [Required, Display(Name = "Brand")] public string Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        
        [Required, MaxLength(30)] 
        public string Title { get; set; } = null!;

        [Required, Display(Name = "Base price"), Range(1, 1000)]
        public double Price { get; set; }

        [Required, Display(Name = "Sale price"), Range(1, 1000)]
        public double DiscountPrice { get; set; }
        
        [Required] public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId"), ValidateNever]
        public Category Category { get; set; } = null!;
        
        [ValidateNever] public List<ProductImage> Images { get; set; } = null!;
        
        public bool ImagesAreValid => Images != null && Images.Count > 0;
        public string ThumbnailUrl => Images?.OrderBy(i => i.DisplayOrder)?.FirstOrDefault()?.Url ?? "https://placehold.co/500x600/png";
    }
}
