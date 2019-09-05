using Microsoft.AspNetCore.Mvc;
using Bookstore.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utilities.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Bookstore.Controllers {

	[Authorize(AuthenticationSchemes = "bsid")]
	public class OrdersController : BaseController {

		private readonly OrderRepository _orders;
		private readonly CustomerRepository _customers;
		private readonly ProductRepository _products;

		public OrdersController(OrderRepository orders, CustomerRepository customers, ProductRepository products) {
			this._orders = orders;
			this._customers = customers;
			this._products = products;
		}

		[HttpGet]
		public IActionResult List() {
			return this.View(this._orders.Items.Select(o => OrderRepresentation.FromEntity(o)));
		}

		[HttpGet]
		public IActionResult New() {
			NewOrderModel model = new NewOrderModel();
			model.Customers = new List<SelectListItem>(this._customers.Items.ToSelectList(c => c.Name, c => c.Id, null));
			model.Products = new List<SelectListItem>(this._products.Items.ToSelectList(p => p.Name, p => p.Id, null));
			return this.View(model);
		}

		[HttpPost]
		public async Task<IActionResult> New(NewOrderModel model) {
			Customer customer = await this._customers.GetByIdAsync(model.CustomerId);
			Product product = await this._products.GetByIdAsync(model.ProductId);
			Order order = await this._orders.AddAsync(customer, model.Description, DateTime.Now);
			this._orders.AddLine(order, product, model.Amount);
			return this.RedirectToAction("List");
		}

		[HttpPost]
		public async Task<IActionResult> Remove(string id) {
			await this._orders.Delete(id);
			return this.RedirectToAction("List");
		}

		[HttpGet]
		public IActionResult Detail(string id) {
			var order = this._orders.GetById(id);
			return this.View(OrderRepresentation.FromEntity(order));
		}

	}

}
