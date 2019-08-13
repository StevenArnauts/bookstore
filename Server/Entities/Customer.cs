﻿using System.Collections.Generic;

namespace Bookstore.Entities {

	public class Customer : Entity {

		public Customer() {
			this.Orders = new List<Order>();
		}

		public string Name { get; set; }
		public ICollection<Order> Orders { get; set; }

	}

}