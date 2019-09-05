using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Bookstore.Models {

	public class NewOrderModel {

		public List<SelectListItem> Customers { get; set; }
		public List<SelectListItem> Products { get; set; }
		public string CustomerId { get; set; }
		public string ProductId { get; set; }
		public int Amount { get; set; }
		public string Description { get; set; }

	}

}