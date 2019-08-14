using System.Collections.Generic;
using IdentityServer4.Models;

namespace Bookstore.Identity {
	public class ApiResources {

		private readonly List<ApiResource> _apiResources = new List<ApiResource> {
			new ApiResource {
				Name = "Archive",
				DisplayName = "Doccle Archive",
				Scopes = new List<Scope> {
					new Scope {
						Name = "documents",
						DisplayName = "Doccle Documents",
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
