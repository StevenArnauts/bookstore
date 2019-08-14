using System.Collections.Generic;

namespace Bookstore.Identity.Models {
	public class ConsentListModel {

		public ConsentListModel() {
			this.Consents = new List<ConsentModel>();
		}

		public IEnumerable<ConsentModel> Consents { get; set; }

	}

}