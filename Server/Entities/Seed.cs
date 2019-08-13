using Utilities.Logging;

namespace Bookstore.Entities {

	public class Seed {

		private readonly OrderRepository _orders;
		private readonly CustomerRepository _customers;
	

		public Seed(OrderRepository orders, CustomerRepository customers) {
			this._orders = orders;
			this._customers = customers;
	
		}

		public void Run() {
			var kbc = this._customers.Add("KBC", "kbc");
			var telenet = this._customers.Add("Telenet");
			Logger.Info(this, "Seeded");
		}

	}

}