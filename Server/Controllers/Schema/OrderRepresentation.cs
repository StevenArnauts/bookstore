using Bookstore.Entities;

namespace Bookstore.Controllers {

	public class OrderRepresentation : OrderSpecification {

		public static OrderRepresentation FromEntity(Order order) {
			return new OrderRepresentation {
				Id = order.Id,
				Amount = order.Amount,
				Date = order.Date,
				Number = order.Number,
				Description = order.Description
			};
		}

		public string Id { get; set; }

	}

}