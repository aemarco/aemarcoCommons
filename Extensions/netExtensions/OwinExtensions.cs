using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Extensions.netExtensions
{
    public static class OwinExtensions
    {




        public static T GetClaimValue<T>(this IPrincipal principal, string type, T defaultValue = default)
        {
            if (principal?.Identity == null) return defaultValue;
            
            var identity = (ClaimsIdentity) principal.Identity;
            object value = identity.FindFirst(type)?.Value;

            if (value == null) return defaultValue;
           
            return (T) Convert.ChangeType(value, typeof(T));
        }

        public static string GetClaimValue(this IPrincipal principal, string type, string defaultValue = default)
        {
            return principal.GetClaimValue<string>(type, defaultValue);
        }


        public static void AddFlag(this ClaimsIdentity identity, string flagName, bool value = true)
        {
            identity.AddClaim(new Claim(flagName, value.ToString()));

        }

        public static bool HasFlag(this IPrincipal principal, string type)
        {
            return principal.GetClaimValue<bool>(type);
        }



    }
}
