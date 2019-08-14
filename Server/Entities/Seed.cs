using System.Linq;
using Utilities.Logging;

namespace Bookstore.Entities {

	public class Seed {

		private readonly OrderRepository _orders;
		private readonly CustomerRepository _customers;
		private readonly BookstoreContext _context;

		public Seed(BookstoreContext context) {
			this._orders = new OrderRepository(context);
			this._customers = new CustomerRepository(context);
			this._context = context;
		}

		public void Run() {
			this._context.Database.EnsureCreated();
			if(!this._customers.Items.Any(c => c.Id == "kbc")) {
				this._customers.Add("KBC", "kbc");
			}
			if (!this._customers.Items.Any(c => c.Name == "Telenet")) {
				this._customers.Add("Telenet");
			}
			Logger.Info(this, "Seeded");
		}

	}

}