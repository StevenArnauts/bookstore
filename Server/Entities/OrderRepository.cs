using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class OrderRepository : IRepository {

		private readonly List<Order> _orders = new List<Order>();

		public Order Add(string id, string description, DateTime date, decimal amount) {
			if (this._orders.Any(a => a.Id == id)) {
				throw new Exception("Order " + description + " already exists");
			}
			Order entity = new Order { Id = id, Description = description, Date = date, Amount = amount };
			this._orders.Add(entity);
			return entity;
		}

		public IEnumerable<Order> Senders {
			get {
				return this._orders.AsReadOnly();
			}
		}

		public Order GetById(string id) {
			return this._orders.Get(u => u.Id == id);
		}

	}

}