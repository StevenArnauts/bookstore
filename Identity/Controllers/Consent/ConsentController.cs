using System.Linq;
using System.Threading.Tasks;
using Bookstore.Identity.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Utilities.Extensions;

namespace Bookstore.Identity.Controllers.Consent {

	/// <summary>
	/// This controller processes the consent UI
	/// </summary>
	[SecurityHeaders]
	[Authorize]
	public class ConsentController : Controller {
		private readonly ConsentService _consent;

		public ConsentController(IIdentityServerInteractionService interaction, IClientStore clientStore, IResourceStore resourceStore, ILogger<ConsentController> logger) {
			this._consent = new ConsentService(interaction, clientStore, resourceStore, logger);
		}

		/// <summary>
		/// Shows the consent screen
		/// </summary>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Index(string returnUrl) {
			ConsentViewModel vm = await this._consent.BuildViewModelAsync(returnUrl);
			if (vm != null) {
				return this.View("Index", vm);
			}
			return this.View("Error");
		}

		/// <summary>
		/// Handles the consent screen postback
		/// </summary>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(ConsentInputModel model) {
			ProcessConsentResult result = await this._consent.ProcessConsent(model);
			if (result.IsRedirect) {
				return this.Redirect(result.RedirectUri);
			}
			if (result.HasValidationError) {
				this.ModelState.AddModelError("", result.ValidationError);
			}
			if (result.ShowView) {
				return this.View("Index", result.ViewModel);
			}
			return this.View("Error");
		}

		[HttpGet]
		public async Task<IActionResult> List() {
			var consents = await this._consent.GetConsents();
			ConsentListModel model = new ConsentListModel {
				Consents = consents.Select(c => new ConsentModel { Client = c.ClientId, Scopes = c.Scopes.Print(", "), Timestamp = c.CreationTime })
			};
			return this.View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Revoke(string clientId) {
			await this._consent.Revoke(clientId);
			return RedirectToAction("List");
		}

	}

}