using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Utilities.Mvc {

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class BasicAuthorizeAttribute : TypeFilterAttribute {

		public BasicAuthorizeAttribute(string realm = null)	: base(typeof(BasicAuthorizeFilter)) {
			this.Arguments = new object[] {
				realm
			};
		}

	}

	public class BasicAuthorizeFilter : IAuthorizationFilter {

		private readonly string _realm;

		public BasicAuthorizeFilter(string realm = null) {
			this._realm = realm;
		}

		public void OnAuthorization(AuthorizationFilterContext context) {
			string authHeader = context.HttpContext.Request.Headers["Authorization"];
			if (authHeader != null && authHeader.StartsWith("Basic ")) {
				// Get the encoded username and password
				string encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
				// Decode from Base64 to string
				string decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
				// Split username and password
				string username = decodedUsernamePassword.Split(':', 2)[0];
				string password = decodedUsernamePassword.Split(':', 2)[1];
				// Check if login is correct
				if (this.IsAuthorized(username, password)) {
					return;
				}
			}
			// Return authentication type (causes browser to show login dialog)
			context.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";
			// Add realm if it is not null
			if (!string.IsNullOrWhiteSpace(this._realm)) {
				context.HttpContext.Response.Headers["WWW-Authenticate"] += $" realm=\"{this._realm}\"";
			}
			// Return unauthorized
			context.Result = new UnauthorizedResult();
		}

		// Make your own implementation of this
		public bool IsAuthorized(string username, string password) {
			// Check that username and password are correct
			return username.Equals("User1", StringComparison.InvariantCultureIgnoreCase)
				   && password.Equals("SecretPassword!");
		}

	}

}