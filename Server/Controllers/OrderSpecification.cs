using System;
using System.Collections.Generic;

namespace Bookstore.Controllers {

	public class OrderSpecification {

		public DateTime Date { get; set; }
		public string Number { get; set; }
		public string Description { get; set; }
		public ICollection<OrderLineSpecification> Lines { get; set; }

	}

}