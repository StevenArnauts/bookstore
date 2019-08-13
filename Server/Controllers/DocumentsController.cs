using System.Collections.Generic;
using Bookstore.Entities;
using Microsoft.AspNetCore.Mvc;
using ViewDocument = Bookstore.Models.Document;

namespace Bookstore.Controllers {

	public class DocumentsController : BaseController {

		private readonly DocumentRepository _documents;

		public DocumentsController(AccountRepository accounts, DocumentRepository documents) : base(accounts) {
			this._documents = documents;
		}

		public IActionResult Archive() {
			List<ViewDocument> docs = new List<ViewDocument>();
			Account account = this.GetAccount();
			foreach (var receiver in account.Receivers) {
				foreach (var document in receiver.Documents) {
					docs.Add(new ViewDocument {
						Uuid = document.Uuid,
						Uri = document.Uri,
						IsRead = document.IsRead,
						Receiver = receiver.Name,
						Sender = receiver.Sender.Name,
						Timestamp = document.Timestamp,
						Title = document.Title
					});
				}
			}
			return this.View(docs);
		}

	}

}