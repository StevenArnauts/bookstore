using System.Collections.Generic;
using IdentityServer4.Models;

namespace Bookstore.Identity {

	public class Clients {

		private readonly HostConfiguration _host;
		private readonly List<Client> _clients;

		public Clients(HostConfiguration host) {
			this._host = host;
			string baseUrl = "http://" + host.Host;
			string baseUrls = "https://" + host.Host;
			this._clients = new List<Client> {
				new Client {
					ClientId = "bookstore.orders",
					ClientName = "Order Service",
					Description = "Bookstore Order Service",
					ClientSecrets = new List<Secret> {
						new Secret("secret".Sha256())
					},
					RequirePkce = false,
					AllowAccessTokensViaBrowser = true,
					AllowedCorsOrigins = new List<string> {
						baseUrl + ":6001",
						baseUrls + ":6101"
					},
					AllowedScopes = new List<string> {
						"openid",
						"profile",
						"email"
					},
					AllowedGrantTypes = GrantTypes.Code,
					AllowOfflineAccess = true,
					RequireConsent = false,
					Enabled = true,
					EnableLocalLogin = true,
					RedirectUris = new List<string> {
						baseUrl + ":6001/callback",
						baseUrls + ":6101/callback"
					},
					PostLogoutRedirectUris = new List<string> {
						baseUrl + ":6001",
						baseUrls + ":6101",
						baseUrls + ":6101/signout-callback"
					}
				}
			};
		}		

		public List<Client> Get() {
			return this._clients;
		}

	}

}
