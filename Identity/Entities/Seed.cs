using System.Linq;
using Utilities.Entities;
using Utilities.Logging;

namespace Bookstore.Identity.Entities {

	public class Seed : ISeed {
		
		private readonly UserRepository _users;
		private readonly IdentityContext _context;

		public Seed(IdentityContext context) {
			this._users = new UserRepository(context);
			this._context = context;
		}

		public void Run() {
			this._context.Database.EnsureCreated();
			const string johnId = "john";
			if (!this._context.Users.Any(u => u.Id == johnId)) {
				this._users.Add(johnId, "John", "john.doe@pencil42.be", "john");
				this._context.SaveChanges();
			}			
			Logger.Info(this, "Seeded");
		}

	}

}