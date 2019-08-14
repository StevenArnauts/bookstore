using System;

namespace Bookstore.Identity.Models {

	public class ConsentModel {

		public string Client { get; set; }
		public string Scopes { get; set; }
		public DateTime	Timestamp { get; set; }

	}

}