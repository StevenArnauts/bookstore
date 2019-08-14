using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Identity.Entities {

	public class UserRepository : IRepository {

		private readonly List<User> _users = new List<User>();

		public User Add(string name, string email, string password) {
			string id = Guid.NewGuid().ToString("N").ToUpper();
			return this.Add(id, name, email, password);
		}

		public User Add(string id, string name, string email, string password) {
			if (this._users.Any(u => u.Id == id)) throw new Exception("User " + id + " already exists");
			User user = new User { Id = id, Name = name, Email = email, Password = password };
			this._users.Add(user);
			return user;
		}

		public User Get(string id) {
			return this._users.Get(u => u.Id == id);
		}

		internal User GetByName(string name) {
			return this._users.Get(u => u.Name == name);
		}
	}

}