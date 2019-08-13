using System.Collections.Generic;
using System.Linq;
using Utilities;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class ReceiverRepository : IRepository {

		private readonly SenderRepository _senders;

		public ReceiverRepository(SenderRepository senders) {
			this._senders = senders;
		}

		public IEnumerable<Receiver> Receivers => this._senders.Senders.SelectMany(s => s.Receivers);

		public Receiver GetById(string id) {
			return this._senders.Senders.SelectMany(s => s.Receivers).Get(r => r.Id == id);
		}

	}

}