using System.Collections.Generic;
using System.Security.Claims;

namespace Bookstore.Identity.Models {

	public class ProfileModel {

		public ProfileModel() {
			this.Claims = new List<Claim>();
		}

		public List<Claim> Claims { get; set; }

	}

}