using Microsoft.AspNetCore.Identity;
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
    public class ApplicationUser : IdentityUser, IModelBase
    {
        //TODO-1: Research difference between 'required' and [Required]
        //Added note: 'required' causes build to fail when using the new() constructor, 
        // likely meaning that it must be initialized in a parameterless constructor, which may be required by the 'required' keyword
        [Required] public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        //TODO-1: Stop mapping role directly in data table
        public string Role { get; set; }
        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId"), ValidateNever] 
        public Company Company { get; set; }

        [NotMapped]
        public bool Locked { get; set;}

        int IModelBase.Id => int.Parse(Id);
    }
}
