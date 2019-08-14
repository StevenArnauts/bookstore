using Utilities.REST;
using System.Threading;
using System;
using Bookstore.Identity;

namespace Bookstore.Identity.Services {

	public class DoccleClient {

		private readonly HostConfiguration _host;

		public DoccleClient(HostConfiguration host) {
			this._host = host;
		}

		public AccountSpecification CreateAccount(string userId, string name) {
			RestClient client = new RestClient(new Uri("https://" + this._host.Host + ":6101"));
			// RestClient client = new RestClient(new Uri("http://" + this._host.Host + ":6001"));
			AccountSpecification spec = new AccountSpecification {
				UserId = userId,
				Name = name
			};
			var docs = client.Post<AccountSpecification, AccountRepresentation>("api/accounts", spec, CancellationToken.None);
			return docs;

		}

	}

}
