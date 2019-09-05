using Utilities.Entities;

namespace Bookstore.Entities {

	public class OrderLine : Entity {

		public int Sequence { get; set; }
		public int Amount { get; set; }

		public Product Product { get; set; }
		public Order Order { get; set; }

	}

}