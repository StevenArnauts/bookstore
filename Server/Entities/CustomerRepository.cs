using System;
using System.Linq;
using Utilities.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bookstore.Entities {

	public class CustomerRepository : BaseRepository<BookstoreContext, Customer> {

		public CustomerRepository(BookstoreContext context) : base(context) { }

		protected override DbSet<Customer> Set => this.Context.Customers;
		protected override IQueryable<Customer> Query => this.Context.Customers.Include(c => c.Orders);

		public Customer Add(string name, string id = null) {
			if (this.Items.Any(a => a.Name == name)) throw new Exception("Customer " + name + " already exists");
			string i = id??Guid.NewGuid().ToString("N").ToUpper();
			var entity = new Customer { Id = i, Name = name };
			this.Add(entity);
			this.Flush();
			return entity;
		}

		public async Task<Customer> AddAsync(string name, string id = null) {
			if (this.Items.Any(a => a.Name == name)) throw new Exception("Customer " + name + " already exists");
			string i = id ?? Guid.NewGuid().ToString("N").ToUpper();
			var entity = new Customer { Id = i, Name = name };
			await this.AddAsync(entity);
			this.Flush();
			return entity;
		}

	}

}