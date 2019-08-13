using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class Receiver {

		private static readonly Sequence uuids = new Sequence(1);

		private readonly List<Document> _documents = new List<Document>();
		private Account _account;
		private Sender _sender;
		private string _name;
		private string _id;
		private string _secret;
	
		public Receiver(Sender sender, string id, string name, string secret) {
			this._id = id;
			this._sender = sender;
			this._name = name;
			this._secret = secret;
		}

		public Document Add(string id, string title, DateTime timestamp) {
			if (this._documents.Any(a => a.Id == id)) throw new Exception("Document " + id + " already exists");
			var document = new Document {
				Id = id,
				Title = title,
				Timestamp = timestamp,
				Uuid = GuidExtensions.Create((ulong)uuids.Next()).ToString("N").ToUpper(),
				Receiver = this
			};
			this._documents.Add(document);
			return document;
		}

		public void Link(Account account) {
			this._account = account;
			account.Link(this);
		}

		public Document GetById(string id) {
			return this._documents.Get(u => u.Id == id);
		}

		public string Name => this._name;
		public string Secret => this._secret;
		public Account Account => this._account;
		public Sender Sender => this._sender;
		public string Id => this._id;
		public IEnumerable<Document> Documents => this._documents.AsReadOnly();

	}

}