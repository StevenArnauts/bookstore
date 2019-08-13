using System;
using System.Linq;

namespace Bookstore.Entities {

	/// <summary>
	/// TODO: make this a bit generic (or could be an exercise??)
	/// </summary>
	public class OrderRepository : BaseRepository<Order> {

		public Order Add(Customer customer, string description, DateTime date, decimal amount, string id = null) {
			string i = id?? Guid.NewGuid().ToString("N").ToUpper();
			string number = (this.Items.Count() + 1).ToString("D6");
			Order entity = new Order { Id = i, Description = description, Date = date, Amount = amount, Number = number };
			this.Add(entity);
			entity.Customer = customer;
			customer.Orders.Add(entity);
			return entity;
		}

	}

}