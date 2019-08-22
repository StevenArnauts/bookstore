using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers {

	public class AccountController : Controller {

		[AllowAnonymous]
		public async Task Login(string returnUrl = "/") {
			await HttpContext.ChallengeAsync("bsid", new AuthenticationProperties() { RedirectUri = returnUrl });
		}

		[Route("logout")]
		[Authorize]
		public async Task Logout() {
			var url = Url.Action("Index", "Home");
			await HttpContext.SignOutAsync("bsid", new AuthenticationProperties {
				RedirectUri = url
			});
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

	}

}