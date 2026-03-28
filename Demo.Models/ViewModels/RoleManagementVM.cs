using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.Models.ViewModels
{
    public class RoleManagementVM
    {
        public ApplicationUser User { get; set; } = null!;
        public IEnumerable<SelectListItem>? Roles { get; set; }
        public IEnumerable<SelectListItem>? Companies { get; set; }
    }
}