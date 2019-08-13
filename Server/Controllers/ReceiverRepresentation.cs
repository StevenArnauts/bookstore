using Bookstore.Entities;

namespace Bookstore.Controllers {
	public class ReceiverRepresentation {

		public static ReceiverRepresentation FromEntity(Receiver receiver) {
			return new ReceiverRepresentation {
				Id = receiver.Id,
				Name = receiver.Name
			};
		}

		public string Name { get; set; }
		public string Id { get; set; }

	}

}