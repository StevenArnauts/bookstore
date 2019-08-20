using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities.Entities;

namespace Bookstore.Entities {

	public class OrderRepository : BaseRepository<BookstoreContext, Order> {

		public OrderRepository(BookstoreContext context) : base(context) { }

		protected override DbSet<Order> Set => this.Context.Orders;
		protected override IQueryable<Order> Query => this.Context.Orders;

		public Order Add(Customer customer, string description, DateTime date, decimal amount, string id = null) {
			string i = id?? Guid.NewGuid().ToString("N").ToUpper();
			string number = (this.Items.Count() + 1).ToString("D6");
			Order entity = new Order { Id = i, Description = description, Date = date, Amount = amount, Number = number };
			this.Add(entity);
			entity.Customer = customer;
			customer.Orders.Add(entity);
			this.Flush();
			return entity;
		}

		public async Task<Order> AddAsync(Customer customer, string description, DateTime date, decimal amount, string id = null) {
			string i = id ?? Guid.NewGuid().ToString("N").ToUpper();
			string number = (this.Items.Count() + 1).ToString("D6");
			Order entity = new Order { Id = i, Description = description, Date = date, Amount = amount, Number = number };
			await this.AddAsync(entity);
			entity.Customer = customer;
			customer.Orders.Add(entity);
			this.Flush();
			return entity;
		}

	}

}