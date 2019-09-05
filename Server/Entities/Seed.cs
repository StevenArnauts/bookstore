using System.Linq;
using System.Threading.Tasks;
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

		public async Task RunAsync() {
			this._context.Database.EnsureCreated();
			if(!this._context.Customers.Any(c => c.Id == "kbc")) {
				await this._customers.AddAsync("KBC", "kbc");
			}
			if (!this._context.Customers.Any(c => c.Name == "Telenet")) {
				await this._customers.AddAsync("Telenet");
			}
			this._context.SaveChanges();
			Logger.Info(this, "Seeded");
		}

	}

}