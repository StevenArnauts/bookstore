using System;

namespace Bookstore.Entities {

	public class Document {

		public string Uri { get; set; }
		public string Uuid { get; set; }
		public string Id { get; set; }
		public string Title { get; set; }
		public DateTime	Timestamp { get; set; }
		public bool IsRead { get; set; }

		public Receiver Receiver { get; set; }

	}

}