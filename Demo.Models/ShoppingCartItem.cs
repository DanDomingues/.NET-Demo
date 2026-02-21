using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Utility;

namespace Demo.Models
{
    //Describes a shopping cart's item, not the cart itself

    public class ShoppingCartItem : ModelBase, IProductOrderItem
    {
        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }
        public int ProductId { get; set; }
        public string ApplicationUserId { get; set; }

        [ForeignKey("ProductId"), ValidateNever]
        public Product Product { get; set; }

        [ForeignKey("ApplicationUserId"), ValidateNever()]
        public ApplicationUser ApplicationUser { get; set; }
    
        //TODO
        //Less of a to-do, and maybe more of a consideration
        //If there's an expectation for a model to have no actions whatsoever, and only hold data, this should be moved to a controller
        //Likewise, if the dynamic calculation introduces more overhead than adding a whole, unmapped new field would, we should move
        //to have this calculated on update and set to a field

        public double TotalCost
        {
            get
            {
                if(Product == null)
                {
                    return 0;
                }

                if (Count >= 100) return Product.Price100 * Count;
                else if(Count >= 50) return Product.Price50 * Count;
                else return Product.Price * Count;
            }
        }
    }
}
