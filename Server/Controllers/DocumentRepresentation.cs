using Bookstore.Entities;

namespace Bookstore.Controllers {

	public class DocumentRepresentation : DocumentSpecification {

		public static DocumentRepresentation FromEntity(Document document) {
			return new DocumentRepresentation {
				Id = document.Id,
				Title = document.Title,
				Timestamp = document.Timestamp
			};
		}

	}

}