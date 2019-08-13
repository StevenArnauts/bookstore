using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class SenderRepository : IRepository {

		private readonly List<Sender> _senders = new List<Sender>();

		public Sender Add(string id, string name) {
			if (this._senders.Any(a => a.Name == name)) throw new Exception("Sender " + name + " already exists");
			var sender = new Sender { Id = id, Name = name };
			this._senders.Add(sender);
			return sender;
		}

		public IEnumerable<Sender> Senders => this._senders.AsReadOnly();

		public Sender GetById(string id) {
			return this._senders.Get(u => u.Id == id);
		}

	}

}