using System.ComponentModel.DataAnnotations;

namespace Bookstore.Identity.Models {

	public class LoginModel : LoginViewModel {

		[Required]
		public string Username { get; set; }

	}

}