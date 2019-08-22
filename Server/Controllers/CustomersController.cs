using Microsoft.AspNetCore.Mvc;
using Bookstore.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Bookstore.Controllers {

	[Authorize(AuthenticationSchemes = "bsid")]
	public class CustomersController : BaseController {

		private readonly CustomerRepository _customers;

		public CustomersController(CustomerRepository customers) {
			this._customers = customers;
		}

		[HttpGet]
		public IActionResult List() {
			return this.View(this._customers.Items);
		}

		[HttpGet]
		public IActionResult New() {
			return this.View();
		}

		[HttpPost]
		public IActionResult New(CustomerSpecification spec) {
			this._customers.Add(spec.Name);
			return this.RedirectToAction("List");
		}

		[HttpPost]
		public IActionResult Remove(string id) {
			this._customers.Remove(id);
			return this.RedirectToAction("List");
		}

		[HttpGet]
		public IActionResult Detail(string id) {
			var customer = this._customers.GetById(id);
			return this.View(customer);
		}

	}

}
