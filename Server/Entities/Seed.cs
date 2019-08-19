using System.Linq;
using Utilities.Entities;
using Utilities.Logging;

namespace Bookstore.Entities {

	public class Seed : ISeed {

		private readonly CustomerRepository _customers;
		private readonly BookstoreContext _context;

		public Seed(BookstoreContext context) {
			this._customers = new CustomerRepository(context);
			this._context = context;
		}

		public void Run() {
			this._context.Database.EnsureCreated();
			if(!this._context.Customers.Any(c => c.Id == "kbc")) {
				this._customers.Add("KBC", "kbc");
			}
			if (!this._context.Customers.Any(c => c.Name == "Telenet")) {
				this._customers.Add("Telenet");
			}
			this._context.SaveChanges();
			Logger.Info(this, "Seeded");
		}

	}

}