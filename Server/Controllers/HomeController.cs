using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Controllers {

	public class HomeController : Controller {

		public IActionResult Index() {
			return this.View();
		}

	}

}