using Bookstore.Entities;

namespace Bookstore.Controllers {

	public class OrderLineRepresentation : OrderLineSpecification {

		public static OrderLineRepresentation FromEntity(OrderLine line) {
			return new OrderLineRepresentation {
				Amount = line.Amount,
				ProductId = line.Product.Id,
				ProductName = line.Product.Name,
				Id = line.Id,
				Sequence = line.Sequence
			};
		}

		public string Id { get; set; }
		public int Sequence { get; set; }
		public string ProductName { get; set; }

	}

}