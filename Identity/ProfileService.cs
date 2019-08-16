using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookstore.Identity.Entities;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Utilities.Extensions;
using Utilities.Logging;

namespace Bookstore.Identity {

	public class ProfileService : IProfileService {

		private readonly UserRepository repository;

		public ProfileService(UserRepository repository) {
			this.repository = repository;
		}

		public Task GetProfileDataAsync(ProfileDataRequestContext context) {
			string sub = context.Subject.Identity.GetSubjectId();
			User identity = this.repository.Get(sub);
			if (identity != null) {
				context.IssuedClaims.AddRange(context.Subject.Claims);
				List<string> claimTypes = new List<string>(context.RequestedClaimTypes);
				claimTypes.AddRange(context.RequestedResources.ApiResources.SelectMany(r => r.Scopes).SelectMany(s => s.UserClaims));
				claimTypes.AddRange(context.RequestedResources.IdentityResources.SelectMany(r => r.UserClaims));
				foreach (var claim in this.GetClaims(identity)) {
					if (!context.IssuedClaims.Any(c => c.Type == claim.Type && c.Value == claim.Value) && claimTypes.Any(t => t == claim.Type)) {
						context.IssuedClaims.Add(claim);
					}
				}
				Logger.Debug(this, context.IssuedClaims.GroupBy(c => c.Type).Print(g => g.Key + "(" + g.Count() + ") "));
			} else {
				Logger.Warn(this, "Identity with subject " + sub + " not found");
			}
			return Task.FromResult(0);
		}

		public IEnumerable<Claim> GetClaims(User user) {
			return new List<Claim> {
				new Claim("sub", user.Id),
				new Claim("name", user.Name),
				new Claim("email", user.Email)
			};
		}
		public Task IsActiveAsync(IsActiveContext context) {
			context.IsActive = true; // avoid looking in accounts and users table
			return Task.FromResult(0);
		}

	}

}
