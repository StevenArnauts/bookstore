using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	/// <summary>
	/// External document storage provider
	/// </summary>
	public class StorageProvider {

		public string Name { get; set; }


	}

	public class Sender {

		private readonly List<Receiver> _receivers = new List<Receiver>();

		public Receiver Add(string id, string name, string secret) {
			if (this._receivers.Any(a => a.Id == id)) throw new Exception("Receiver " + id + " already exists");
			var receiver = new Receiver(this, id, name, secret);
			this._receivers.Add(receiver);
			return receiver;
		}

		public Receiver GetByName(string name) {
			return this._receivers.Get(u => u.Name == name);
		}

		public string Id { get; set; }
		public string Name { get; set; }

		public IEnumerable<Receiver> Receivers => this._receivers.AsReadOnly();

	}

}