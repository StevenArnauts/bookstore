using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities.Entities;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class OrderRepository : BaseRepository<BookstoreContext, Order> {

		public OrderRepository(BookstoreContext context) : base(context) { }

		protected override DbSet<Order> Set => this.Context.Orders;
		protected override IQueryable<Order> Query => this.Context.Orders.Include(o => o.Lines).ThenInclude(l => l.Product);

		public async Task<Order> AddAsync(Customer customer, string description, DateTime date, string id = null) {
			string i = id ?? this.NewId();
			string number = (this.Items.Count() + 1).ToString("D6");
			Order entity = new Order { Id = i, Description = description, Date = date, Number = number };
			await this.AddAsync(entity);
			entity.Customer = customer;
			customer.Orders.Add(entity);
			this.Flush();
			return entity;
		}

		public OrderLine AddLine(Order order, Product product, int amount) {
			int sequence = order.Lines.Any() ? order.Lines.Max(l => l.Sequence) + 1 : 1;
			OrderLine line = new OrderLine {
				Id = this.NewId(),
				Product = product,
				Amount = amount,
				Sequence = sequence
			};
			order.Lines.Add(line);
			order.Amount += product.Price * amount;
			this.Flush();
			return line;
		}

		public async Task Delete(string id) {
			Order order = await this.GetByIdAsync(id);
			order.Lines.ForEach(l => this.Context.Lines.Remove(l));
			this.Context.Orders.Remove(order);
			await this.Context.SaveChangesAsync();
		}

	}

}