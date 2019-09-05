using Bookstore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Controllers {

	public class OrderRepresentation : OrderSpecification {

		public static OrderRepresentation FromEntity(Order order) {
			return new OrderRepresentation {
				Id = order.Id,
				Amount = order.Amount,
				Date = order.Date,
				Number = order.Number,
				Description = order.Description,
				Lines = order.Lines.Select(l => OrderLineRepresentation.FromEntity(l)).ToList()
			};
		}

		public string Id { get; set; }
		public decimal Amount { get; set; }
		public new List<OrderLineRepresentation> Lines { get; set; }

	}

}