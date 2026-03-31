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
    public class OrderHeader : ModelBase
    {
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateOnly PaymentDueDate { get; set; }
        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        [Required] public string PhoneNumber { get; set; } = null!;
        [Required] public string StreetAddress { get; set; } = null!;
        [Required] public string City { get; set; } = null!;
        [Required] public string State { get; set; } = null!;
        [Required] public string PostalCode { get; set; } = null!;
        [Required] public string FirstName { get; set; } = null!;
        [Required] public string LastName { get; set; } = null!;

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey("ApplicationUserId"), ValidateNever]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}";
    }
}
