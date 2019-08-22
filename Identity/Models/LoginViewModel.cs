using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Identity.Models {

	public class LoginViewModel : LoginInputModel {

		public LoginViewModel() {
			this.ExternalProviders = new List<ExternalProvider>();
		}

		public bool AllowRememberLogin { get; set; }
		public bool EnableLocalLogin { get; set; }

		public IEnumerable<ExternalProvider> ExternalProviders { get; set; }
		public IEnumerable<ExternalProvider> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

		public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
		public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

	}

}
