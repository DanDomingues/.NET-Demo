using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northstar.Models
{
    public class OrderHeader : ModelBase, IShippingContainer
    {
        [Display(Name = "Order Date")] public DateTime OrderDate { get; set; }
        [Display(Name = "Shipping Date")] public DateTime ShippingDate { get; set; }
        [Display(Name = "Payment Date")] public DateTime PaymentDate { get; set; }
        [Display(Name = "Payment Due Date")] public DateOnly PaymentDueDate { get; set; }
        [Display(Name = "Order Total")] public double OrderTotal { get; set; }


        [Display(Name = "Order Status")] public string? OrderStatus { get; set; } = null!;
        [Display(Name = "Payment Status")] public string? PaymentStatus { get; set; } = null!;
        [Display(Name = "Tracking Number")] public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }

        [Required, Display(Name = "First Name")] public string FirstName { get; set; } = null!;
        [Required, Display(Name = "Last Name")] public string LastName { get; set; } = null!;
        [Required, Display(Name = "Phone Number")] public string? PhoneNumber { get; set; } = null!;
        [Required, Display(Name = "Street Address")] public string? StreetAddress { get; set; } = null!;
        [Required] public string? City { get; set; } = null!;
        [Required] public string? State { get; set; } = null!;
        [Required, Display(Name = "Postal Code")] public string? PostalCode { get; set; } = null!;

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey("ApplicationUserId"), ValidateNever]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
