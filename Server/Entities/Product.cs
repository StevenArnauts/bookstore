using Utilities.Entities;

namespace Bookstore.Entities {

	public class Product : Entity {

		public string Name { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }

	}

}