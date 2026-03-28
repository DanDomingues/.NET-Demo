using System.Security.Claims;
using System.Security.Principal;

namespace Demo.Utility
{
    public static class RoleUtility
    {
        public static bool IsInRoles(this ClaimsPrincipal user, params string[] roles)
        {
            return roles.Any(user.IsInRole);
        }

        public static bool HasAdminRights(this ClaimsPrincipal user)
        {
            return user.IsInRoles(SD.ROLE_USER_ADMIN, SD.ROLE_USER_EMPLOYEE);
        }

        public static bool TryGetId(this IPrincipal user, out string id)
        {
            var claimsIdentity = user.Identity as ClaimsIdentity;            
            id = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            
            if(string.IsNullOrEmpty(id))
            {
                return false;
            }
            
            return true;
        }
    }
}
