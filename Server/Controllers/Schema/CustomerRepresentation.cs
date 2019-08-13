using Bookstore.Entities;

namespace Bookstore.Controllers {

	public class CustomerRepresentation: CustomerSpecification {

		public string Id { get; set; }

		public static CustomerRepresentation FromEntity(Customer customer) {
			return new CustomerRepresentation {
				Id = customer.Id,
				Name = customer.Name
			};
		}

	}

}
