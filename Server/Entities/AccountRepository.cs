using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class AccountRepository : IRepository {

		private readonly List<Account> _accounts = new List<Account>();

		public Account Add(string name, string userId) {
			if (this._accounts.Any(a => a.Name == name)) throw new Exception("Account " + name + " already exists");
			var account = new Account { Name = name, UserId = userId };
			this._accounts.Add(account);
			return account;
		}

		public Account GetByUserId(string id) {
			return this._accounts.Get(u => u.UserId == id);
		}

		public Account GetByName(string name) {
			return this._accounts.Get(u => u.Name == name);
		}

	}

}