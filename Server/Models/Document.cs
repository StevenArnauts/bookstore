using System;

namespace Bookstore.Models {

	public class Document {

		public string Uuid { get; set; }
		public string Uri { get; set; }
		public string Title { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsRead { get; set; }
		public string Receiver { get; set; }
		public string Sender { get; set; }

	}

}
