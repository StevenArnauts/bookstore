using System.Collections.Generic;
using Bookstore.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers {

	public class OrdersController : BaseController {

		private readonly OrderRepository _orders;

		public OrdersController(OrderRepository orders) {
			this._orders = orders;
		}

		public IActionResult List() {
			List<Order> orders = new List<Order>();
			return this.View(orders);
		}

	}

}