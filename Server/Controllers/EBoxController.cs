using System.Linq;
using Common;
using Bookstore.Entities;
using Bookstore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utilities.Logging;

namespace Bookstore.Controllers {

	public class EBoxController: BaseController {

		private readonly AccountRepository _accounts;
		private readonly HostConfiguration _host;

		public EBoxController(AccountRepository accounts, HostConfiguration host) : base(accounts) {
			this._accounts = accounts;
			this._host = host;
		}

		[Authorize(AuthenticationSchemes = "dora")]
		public IActionResult Settings() {
			var account = this.GetAccount();
			EBoxSettings model = new EBoxSettings();
			model.IsLinked = account.Receivers.Any(r => r.Name == "ebox");
			return this.View(model);
		}

		[Authorize(AuthenticationSchemes = "dora")]
		public IActionResult Link([FromQuery]string returnUrl) {

			Logger.Info(this, "Linking ebox...");

			// check if not already linked
			var account = this.GetAccount();
			if (account.Receivers.Any(r => r.Name == "ebox")) {
				Logger.Info(this, "Already linked to ebox");
				return this.Redirect(returnUrl);
			}
			// string url = "http://" + this._host.Host + ":6004/EBox/Link?returnUrl=" + returnUrl;
			string url = "https://" + this._host.Host + ":6104/ebox/link?returnurl=" + returnUrl;
			Logger.Debug(this, "Redirecting to " + url);
			return base.Redirect(url);
		}

	}

}
