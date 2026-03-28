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
        [Required] public string Name { get; set; } = null!;
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        //TODO-1: Stop mapping role directly in data table
        public string Role { get; set; } = null!;
        public int? CompanyId { get; set; }

        [ForeignKey("CompanyId"), ValidateNever] 
        public Company? Company { get; set; }

        [NotMapped]
        public bool Locked { get; set;}

        int IModelBase.Id => int.Parse(Id);
    }
}
