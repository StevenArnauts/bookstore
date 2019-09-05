using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookstore.Controllers;
using Bookstore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Resources {

	[Authorize(AuthenticationSchemes = "bsoa")]
	[Route("api")]
	[ApiController]
	public class CustomerResource : ControllerBase {

		private readonly CustomerRepository _customers;
		private readonly OrderRepository _orders;
		private readonly ProductRepository _products;

		public CustomerResource(CustomerRepository customers, OrderRepository orders, ProductRepository products) {
			this._customers = customers;
			this._orders = orders;
			this._products = products;
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
			var order = await this._orders.AddAsync(customer, spec.Description, spec.Date);
			foreach(OrderLineSpecification line in spec.Lines) {
				Product product = await this._products.GetByIdAsync(line.ProductId);
				this._orders.AddLine(order, product, line.Amount);
			}
			return this.Ok(OrderRepresentation.FromEntity(order));
		}

	}

}