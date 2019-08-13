using Microsoft.AspNetCore.Mvc;
using Bookstore.Entities;

namespace Bookstore.Controllers {

	[Route("customers")]
	public class CustomersController : BaseController {

		private readonly CustomerRepository _customers;

		// [Authorize(AuthenticationSchemes = "dora")]
		public CustomersController(CustomerRepository customers) {
			this._customers = customers;
		}

		public IActionResult List() {
			return this.View(this._customers.Customers);
		}

		[Route("detail/{id}")]
		public IActionResult Detail(string id) {
			return this.View(this._customers.Customers);
		}

	}

}
