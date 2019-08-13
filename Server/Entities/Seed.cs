using System;

namespace Bookstore.Entities {

	public class Seed {

		private readonly AccountRepository _accounts;
		private readonly SenderRepository _senders;
	

		public Seed(AccountRepository accounts, SenderRepository senders) {
			this._accounts = accounts;
			this._senders = senders;
	
		}

		public void Run() {

			var kbc = this._senders.Add("kbc", "KBC");
			var telenet = this._senders.Add("telenet", "Telenet");
			var ebox = this._senders.Add("ebox", "eBox");

			var steven = this._accounts.Add("Steven", "sar");
			var mattias = this._accounts.Add("Mattias", "mgi");

			var stevenkbc = kbc.Add("steven-kbc", "KBC Documenten van Steven", "abc");
			stevenkbc.Link(steven);
			AddDocument(stevenkbc, "doc-001", "Rekeninguittreksels 2019-03", DateTime.Now.AddDays(-10));
			AddDocument(stevenkbc, "doc-002", "Vervaldagbericht Woningpolis", DateTime.Now.AddDays(-45));
			AddDocument(stevenkbc, "doc-003", "Vervaldagbericht Woningpolis", DateTime.Now.AddDays(-45));

			var mattiastelenet = telenet.Add("mattias-telenet", "Telenet documenten", "abc");
			mattiastelenet.Link(mattias);
			AddDocument(mattiastelenet, "doc-001", "Telenet April 2019", DateTime.Now.AddDays(-10));
			AddDocument(mattiastelenet, "doc-002", "Telenet Maart 2019", DateTime.Now.AddDays(-45));

		}

		private void AddDocument(Receiver receiver, string id, string title, DateTime timestamp) {

			Document document = receiver.Add(id, title, timestamp);

			//byte[] content = this._printer.Print(new Services.Printing.Document {
			//	Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum elementum nisi non metus aliquam, a blandit odio mollis. Nullam venenatis, augue vitae fringilla tempor, odio diam faucibus tellus, vitae sagittis nulla risus in sem. Maecenas faucibus hendrerit lectus et ultricies. Vestibulum urna felis, lobortis eu bibendum eu, efficitur eget dui. Sed nisl ipsum, consectetur eget porttitor id, varius nec libero. Mauris commodo consectetur lorem, sit amet congue metus auctor vel. Donec eu vehicula purus. Vestibulum ac malesuada nibh, ut mattis nisl.",
			//	Id = document.Uuid,
			//	Receiver = document.Receiver.Name,
			//	Sender = document.Receiver.Sender.Name,
			//	Timestamp = document.Timestamp,
			//	Title = document.Title
			//}).Result;

			//string uri = this._store.Store(new Storage.Document {
			//	Content = content,
			//	Id = id,
			//	Receiver = receiver.Id,
			//	Sender = receiver.Sender.Id
			//});
			// document.Uri = uri;

		}

	}

}