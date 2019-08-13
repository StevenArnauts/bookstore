using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class DocumentRepository : IRepository {

		private readonly ReceiverRepository _receivers;

		public DocumentRepository(ReceiverRepository receivers) {
			this._receivers = receivers;
		}

		public Document GetByUri(string uri) {
			return this._receivers.Receivers.SelectMany(s => s.Documents).Get(r => r.Uri == uri.ToLower());
		}

		public Document GetByUuid(string uuid) {
			return this._receivers.Receivers.SelectMany(s => s.Documents).Get(r => r.Uuid == uuid.ToUpper());
		}

	}

}