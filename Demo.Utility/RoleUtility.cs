using System.Security.Claims;

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
    }
}
