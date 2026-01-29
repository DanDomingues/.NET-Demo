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
        [Required] public string ISBN { get; set; }
        [Required] public string Author { get; set; }
        public string Description { get; set; }
        
        [Required, MaxLength(30)] 
        public string Title { get; set; }  

        [Required, Display(Name = "Regular price"), Range(1, 1000)]
        public double Price { get; set; }

        [Required, Display(Name = "Price for 50+"), Range(1, 1000)]
        public double Price50 { get; set; }

        [Required, Display(Name = "Price for 100+"), Range(1, 1000)]
        public double Price100 { get; set; }

        [Required] 
        public int TotallyNotAnID { get; set; }
        
        [Required] 
        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId"), ValidateNever]
        public Category Category { get; set; }
        
        public string ImageUrl {  get; set; }
    }
}
