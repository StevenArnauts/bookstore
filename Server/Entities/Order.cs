using System;
using Utilities.Entities;

namespace Bookstore.Entities {

	public class Order : Entity {

		public DateTime Date { get; set; }
		public string Number { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }
		public Customer Customer { get; set; }

	}

}