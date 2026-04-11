using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northstar.Models.ViewModels
{
    public class ManageRoleVM
    {
        public ApplicationUser User { get; set; } = null!;
        public IEnumerable<SelectListItem>? Roles { get; set; }
        public IEnumerable<SelectListItem>? Companies { get; set; }
    }
}