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
    public class ApplicationUser : IdentityUser, IModelBase, INamedAddressContainer
    {
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId"), ValidateNever] 
        public Company? Company { get; set; }

        [NotMapped]
        public string Role { get; set; } = null!;

        [NotMapped]
        public bool Locked { get; set;}

        [Display(Name = "Full Name")]        
        public string FullName => $"{FirstName} {LastName}";

        int IModelBase.Id => int.Parse(Id);
    }
}
