using System.Collections.Generic;
using IdentityServer4.Models;

namespace Bookstore.Identity {

	public class ApiResources {

		private readonly List<ApiResource> _apiResources = new List<ApiResource> {
			new ApiResource {
				Name = "bookstore",
				DisplayName = "Bookstore",
				Scopes = new List<Scope> {
					new Scope {
						Name = "bookstore.orders",
						DisplayName = "Bookstore Orders",
						ShowInDiscoveryDocument = true
					}					
				}
			}
		};

		public List<ApiResource> Get() {
			return this._apiResources;
		}

	}

}
