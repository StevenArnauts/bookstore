using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Entities;

namespace Bookstore.Entities {

	public class Order : Entity {

		public Order() {
			this.Lines = new List<OrderLine>();
		}

		public DateTime Date { get; set; }
		public string Number { get; set; }
		public string Description { get; set; }
		public decimal Amount { get; set; }

		public ICollection<OrderLine> Lines { get; set; }
		public Customer Customer { get; set; }

	}

}