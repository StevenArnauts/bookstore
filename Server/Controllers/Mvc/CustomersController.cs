using Microsoft.AspNetCore.Mvc;
using Bookstore.Entities;

namespace Bookstore.Controllers {

	public class CustomersController : BaseController {

		private readonly CustomerRepository _customers;

		// [Authorize(AuthenticationSchemes = "dora")]
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
