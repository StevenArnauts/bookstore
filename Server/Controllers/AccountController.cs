using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Bookstore.Entities;
using Utilities;
using System;

namespace Bookstore.Controllers {

	[Route("account")]
	public class AccountController : BaseController {

		private readonly ReceiverRepository _receivers;

		public AccountController(AccountRepository accounts, ReceiverRepository receivers) : base(accounts) {
			this._receivers = receivers;
		}

		[Route("login")]
		public async Task Login(string returnUrl = "/") {
			await HttpContext.ChallengeAsync("dora", new AuthenticationProperties() { RedirectUri = returnUrl });
		}

		[Route("logout")]
		[Authorize]
		public async Task Logout() {
			await HttpContext.SignOutAsync("dora", new AuthenticationProperties {
				RedirectUri = Url.Action("Index", "Home")
			});
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}

		[Route("link")]
		[Authorize]
		public async Task<IActionResult> Link(string receiverId, string secret, string returnUrl) {
			var account = this.GetAccount();
			var receiver = this._receivers.GetById(receiverId);
			Throw<Exception>.When(receiver == null, "Receiver not found");
			Throw<Exception>.When(receiver.Secret != secret, "Secret is wrong");
			receiver.Link(account);
			if (!string.IsNullOrEmpty(returnUrl)) {
				// return this.Redirect(returnUrl + "?receiverId=" + receiverId);
				return this.Redirect(returnUrl);
			} else {
				return this.RedirectToAction("Archive", "Documents");
			}
		}

		[Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }	

	}

}
