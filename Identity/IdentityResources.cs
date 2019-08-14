using System.Collections.Generic;
using IdentityServer4.Models;

namespace Bookstore.Identity {

	public class IdentityResources {

		private readonly List<IdentityResource> _identityResources = new List<IdentityResource> {
			new IdentityResource {
				Name = "roles",
				UserClaims = new List<string> { "role" },
				ShowInDiscoveryDocument = true
			},
			new IdentityServer4.Models.IdentityResources.OpenId(),
			new IdentityServer4.Models.IdentityResources.Profile(),
			new IdentityServer4.Models.IdentityResources.Email()
		};

		public List<IdentityResource> Get() {
			return this._identityResources;
		}

	}

}
