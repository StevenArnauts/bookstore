using Bookstore.Entities;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;

namespace Bookstore.Controllers {

	public abstract class BaseController : Controller {

		private readonly AccountRepository _accounts;

		protected BaseController(AccountRepository accounts) {
			this._accounts = accounts;
		}

		protected Account GetAccount() {
			Account account = this._accounts.GetByUserId(this.GetUserId());
			return account;
		}

		protected string GetUserId() {
			string doraId = this.User.GetUserId();
			return doraId;
		}

	}

}
