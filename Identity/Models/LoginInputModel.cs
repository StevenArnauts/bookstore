﻿using System.ComponentModel.DataAnnotations;

namespace Bookstore.Identity.Models {

	public class LoginInputModel {

		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		public bool RememberLogin { get; set; }
		public string ReturnUrl { get; set; }

	}

}
