using System.Collections.Generic;
using IdentityServer4.Models;
using Utilities.Web;

namespace Bookstore.Identity {

	public class Clients {

		private readonly WebConfiguration _host;
		private readonly List<Client> _clients;

		public Clients(WebConfiguration host) {
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
						baseUrls
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
						baseUrls + "/callback"
					},
					PostLogoutRedirectUris = new List<string> {
						baseUrl + ":6001",
						baseUrls,
						baseUrls + "/signout-callback"
					}
				},
				new Client {
					ClientId = "postman",
					ClientName = "Postman",
					Description = "Postman Credentials",
					ClientSecrets = new List<Secret> {
						new Secret("secret".Sha256())
					},
					RequirePkce = false,
					AllowedScopes = new List<string> {
						"openid",
						"profile",
						"email",
						"bookstore.orders"
					},
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					Enabled = true
				}
			};
		}		

		public List<Client> Get() {
			return this._clients;
		}

	}

}
