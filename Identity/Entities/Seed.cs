namespace Bookstore.Identity.Entities {

	public class Seed {

		private readonly UserRepository _users;

		public Seed(UserRepository users) {
			this._users = users;
		}

		public void Run() {
			var steven = this._users.Add("sar", "Steven", "steven@pencil42.be", "steven");
		}

	}

}