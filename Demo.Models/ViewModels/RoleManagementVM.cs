using System.ComponentModel.DataAnnotations;

namespace Demo.Models.ViewModels
{
    public class RoleManagementVM
    {
        public ApplicationUser? User { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public IEnumerable<Company>? Companies { get; set; }
    }
}