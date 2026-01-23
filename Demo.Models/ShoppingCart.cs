using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Models
{

    //Describes a shopping cart's item, not the cart itself

    public class ShoppingCart : ModelBase
    {
        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }
        public int ProductId { get; set; }
        public string ApplicationUserId { get; set; }

        [ForeignKey("ProductId"), ValidateNever]
        public Product Product { get; set; }

        [ForeignKey("ApplicationUserId"), ValidateNever()]
        public ApplicationUser ApplicationUser { get; set; }
    
        public double TotalCost
        {
            get
            {
                if (Count >= 100) return Product.Price100 * Count;
                else if(Count >= 50) return Product.Price50 * Count;
                else return Product.Price * Count;
            }
        }
    }
}
