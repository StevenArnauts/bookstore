using System.Linq;
using System.Threading.Tasks;
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

		public async Task RunAsync() {
			await this._context.Database.EnsureCreatedAsync();
			const string johnId = "john";
			if (!this._context.Users.Any(u => u.Id == johnId)) {
				this._users.Add(johnId, "John", "john.doe@pencil42.be", "john");
				await this._context.SaveChangesAsync();
			}			
			Logger.Info(this, "Seeded");
		}

	}

}