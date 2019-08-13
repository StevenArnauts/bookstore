using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Utilities.Logging;

namespace Utilities.Extensions {

	public static class ClaimsPrincipalExtensions {

		public const string ROLE_CLAIM_TYPE = "tml-role";
	    public const string SUB_CLAIM_TYPE = "sub";

        public const string ADMIN_CLAIM_VALUE = "tml-admin";
	    public const string USER_CLAIM_VALUE = "tml-user";

        public static IEnumerable<Claim> FindClaims(this ClaimsPrincipal principal, string type) {
			if (principal == null) {
				throw new ArgumentNullException(nameof(principal));
			}
			IEnumerable<Claim> claims = principal.Claims.Where(c => c.Type == type);
			return (claims);
		}

		public static IEnumerable<string> FindClaimValues(ClaimsPrincipal principal, string type) {
			return (principal.FindClaims(type).Select(c => c.Value));
		}

		public static bool HasClaim(this ClaimsPrincipal principal, string type, string value) {
			if (principal == null) {
				throw new ArgumentNullException(nameof(principal));
			}
			IEnumerable<Claim> claims = principal.FindClaims(type);
			if (!claims.Any(c => c.Value == value)) {
				Logger.Debug(typeof (ClaimsPrincipal), "Principal " + principal.Identity.Name + " misses required claim " + type + " but has these claims: " + principal.Claims.Print(c => c.Type + "=" + c.Value + " "));
				return (false);
			}
			return (true);
		}

		public static bool IsInRole(this ClaimsPrincipal principal, string role) {
			return (principal.HasClaim(ROLE_CLAIM_TYPE, role));
		}

		public static bool IsAdmin(this ClaimsPrincipal principal)
		{
			return (principal.HasClaim(ROLE_CLAIM_TYPE, ADMIN_CLAIM_VALUE));
		}

	    public static string GetUserId(this ClaimsPrincipal principal)
	    {
	        var result = FindClaimValues(principal, SUB_CLAIM_TYPE).SingleOrDefault();
            return result;
	    }

	}

}