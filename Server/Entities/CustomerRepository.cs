using System;
using System.Linq;

namespace Bookstore.Entities {

	public class CustomerRepository : BaseRepository<Customer> {

		public Customer Add(string name, string id = null) {
			if (this.Items.Any(a => a.Name == name)) throw new Exception("Customer " + name + " already exists");
			string i = id??Guid.NewGuid().ToString("N").ToUpper();
			var entity = new Customer { Id = i, Name = name };
			this.Add(entity);
			return entity;
		}

	}

}