using System.Collections.Generic;

namespace Bookstore.Entities {

	public class Account {

		private readonly List<Receiver> _receivers = new List<Receiver>();

		public string Name { get; set; }
		public string UserId { get; set; }

		public string EBoxReceiver { get; set; }
		public string EboxAccessToken { get; set; }
		public string EboxCitizenId { get; set; }

		public IEnumerable<Receiver> Receivers => this._receivers.AsReadOnly();

		public void Link(Receiver receiver) {
			this._receivers.Add(receiver);
		}

	}

}