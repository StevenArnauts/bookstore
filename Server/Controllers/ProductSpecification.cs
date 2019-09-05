namespace Bookstore.Controllers {

	public class ProductSpecification {

		public string Name { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }

	}

	public class ProductRepresentation : ProductSpecification {

		public string Id { get; set; }

	}

}
