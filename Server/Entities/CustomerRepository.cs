using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class CustomerRepository : IRepository {

		private readonly List<Customer> _customers = new List<Customer>();

		public Customer Add(string id, string name) {
			if (this._customers.Any(a => a.Name == name)) throw new Exception("Customer " + name + " already exists");
			var entity = new Customer { Id = id, Name = name };
			this._customers.Add(entity);
			return entity;
		}

		public IEnumerable<Customer> Customers => this._customers.AsReadOnly();

		public Customer GetById(string id) {
			return this._customers.Get(u => u.Id == id);
		}

	}

}