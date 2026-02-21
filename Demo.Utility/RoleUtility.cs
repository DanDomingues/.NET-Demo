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

        public static string GetUserId(this IPrincipal user)
        {
            var claimsIdentity = user.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static bool EqualsAny(this string s, params string[] values)
        {
            return values.Any(s.Equals);
        }
    }
}
