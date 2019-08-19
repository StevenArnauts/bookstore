using Utilities.Entities;

namespace Bookstore.Identity.Entities {

	public class User : Entity {

		public string Name { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }

	}

}