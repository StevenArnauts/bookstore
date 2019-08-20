using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Entities;
using Utilities.Extensions;

namespace Bookstore.Identity.Entities {

	public class UserRepository : BaseRepository<IdentityContext, User> {

		public UserRepository(IdentityContext context) : base(context) { }

		protected override DbSet<User> Set => this.Context.Users;
		protected override IQueryable<User> Query => this.Context.Users;

		public User Add(string name, string email, string password) {
			string id = Guid.NewGuid().ToString("N").ToUpper();
			return this.Add(id, name, email, password);
		}

		public User Add(string id, string name, string email, string password) {
			if (this.Set.Any(u => u.Id == id)) throw new Exception("User " + id + " already exists");
			User user = new User { Id = id, Name = name, Email = email, Password = password };
			this.Set.Add(user);
			return user;
		}

		public User Get(string id) {
			return this.Query.Get(u => u.Id == id);
		}

		internal User GetByName(string name) {
			return this.Query.Get(u => u.Name == name);
		}
	}

}