using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookstore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers {

	[Authorize(AuthenticationSchemes = "bsoa")]
	[Route("api")]
	[ApiController]
	public class ApiController : ControllerBase {

		private readonly CustomerRepository _customers;
		private readonly OrderRepository _orders;

		public ApiController(CustomerRepository customers, OrderRepository orders) {
			this._customers = customers;
			this._orders = orders;
		}		

		[HttpGet]
		[Route("customers")]
		public async Task<ActionResult<IEnumerable<CustomerRepresentation>>> FindCustomers() {
			var customers = await this._customers.QueryAsync(c => true);
			return this.Ok(customers.Select(d => CustomerRepresentation.FromEntity(d)));
		}

		[HttpPost]
		[Route("customers")]
		public async Task<ActionResult<CustomerRepresentation>> CreateCustomer([FromBody] CustomerSpecification spec) {
			var customer = await this._customers.AddAsync(spec.Name);
			return this.Ok(CustomerRepresentation.FromEntity(customer));
		}

		[HttpGet]
		[Route("customers/{customerId}/orders")]
		public async Task<ActionResult<IEnumerable<OrderRepresentation>>> FindOrders([FromRoute] string customerId) {
			var customer = await this._customers.GetByIdAsync(customerId);
			var orders = customer.Orders;
			return this.Ok(orders.Select(d => OrderRepresentation.FromEntity(d)));
		}

		[HttpPost]
		[Route("customers/{customerId}/orders")]
		public async Task<ActionResult<OrderRepresentation>> CreateOrder([FromRoute] string customerId, [FromBody] OrderSpecification spec) {
			var customer = await this._customers.GetByIdAsync(customerId);
			var order = await this._orders.AddAsync(customer, spec.Description, spec.Date, spec.Amount);
			return this.Ok(OrderRepresentation.FromEntity(order));
		}

	}

}