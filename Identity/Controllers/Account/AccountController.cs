using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookstore.Identity.Entities;
using Bookstore.Identity.Models;
using Bookstore.Identity.Services;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;
using Utilities.Extensions;
using Utilities.Logging;

namespace Bookstore.Identity.Controllers
{

	public class ProfileModel {

		public ProfileModel() {
			this.Claims = new List<Claim>();
		}

		public List<Claim> Claims { get; set; }

	}

	public class AccountController: Controller {

		private readonly UserRepository _userRepository;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly IRefreshTokenStore _refreshTokenService;
		private readonly IEventService _events;
		private readonly AccountService _account;

		public AccountController(UserRepository userRepository, IIdentityServerInteractionService interaction, IHttpContextAccessor httpContextAccessor, IClientStore clientStore, IAuthenticationSchemeProvider schemeProvider, IRefreshTokenStore refreshTokenService, IEventService events) {
			this._userRepository = userRepository;
			this._interaction = interaction;
			this._refreshTokenService = refreshTokenService;
			this._events = events;
			this._account = new AccountService(interaction, httpContextAccessor, schemeProvider, clientStore);
		}

		[Authorize(AuthenticationSchemes = Authentication.Scheme)]
		public IActionResult Profile() {
			ProfileModel model = new ProfileModel();
			model.Claims.AddRange(this.User.Claims);
			return View(model);
		}

		[Authorize(AuthenticationSchemes = Authentication.Scheme)]
		public IActionResult Consent() {
			return View();
		}

		[AllowAnonymous]
		public async Task<IActionResult> Login([FromQuery(Name = "ReturnUrl")] string returnUrl) {

			AuthorizationRequest request = await this._interaction.GetAuthorizationContextAsync(returnUrl);

			Console.WriteLine(request?.AcrValues.Print(", "));

			// build a model so we know what to show on the login page
			LoginViewModel vm = await this._account.BuildLoginViewModelAsync(returnUrl);

			return View(vm);

			// return View(new LoginModel { ReturnUrl = returnUrl });
		}
	
		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model) {
			if (ModelState.IsValid) {
				try {
					User user = this._userRepository.GetByName(model.Username);
					if (user.Password == model.Password) {
						ClaimsPrincipal principal = this.CreatePrincipal(user);
						await HttpContext.SignInAsync(Authentication.Scheme, principal);
						return Redirect(string.IsNullOrEmpty(model.ReturnUrl) ? "/" : model.ReturnUrl);
					} else {
						ModelState.AddModelError("Password", "Password is wrong");
					}
				} catch (ObjectNotFoundException) {
					ModelState.AddModelError("Username", "User does not exist");
				}
			}
			return View();
		}

		[AllowAnonymous]
		public IActionResult Register([FromQuery(Name = "ReturnUrl")] string returnUrl) {
			return View(new RegisterModel { ReturnUrl = returnUrl });
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel model) {
			var user = this._userRepository.Add(model.Name, model.Email, model.Password);
			this._userRepository.Flush();
			var principal = this.CreatePrincipal(user);
			await HttpContext.SignInAsync(Authentication.Scheme, principal);
			if(!string.IsNullOrEmpty(model.ReturnUrl)) {
				return Redirect(model.ReturnUrl);
			}
			return RedirectToAction("Index", "Home");
		}
	
		private ClaimsPrincipal CreatePrincipal(User user) {
			var claims = new[] {
				new Claim("name", user.Name),
				new Claim("role", "User"),
				new Claim("sub", user.Id)
			};
			var identity = new ClaimsIdentity(claims, Authentication.Scheme, "name", "role");
			var principal = new ClaimsPrincipal(identity);
			return principal;
		}

		/// <summary>
		///     Show logout page
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId) {
			// build a model so the logout page knows what to display
			LogoutViewModel vm = await this._account.BuildLogoutViewModelAsync(logoutId);

			if (vm.ShowLogoutPrompt == false) {
				// if the request for logout was properly authenticated from IdentityServer, then
				// we don't need to show the prompt and can just log the user out directly.
				return await Logout(vm);
			}

			return View(vm);
		}

		/// <summary>
		///     Handle logout page postback
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> Logout(LogoutInputModel model) {
			// build a model so the logged out page knows what to display
			LoggedOutViewModel vm = await this._account.BuildLoggedOutViewModelAsync(model.LogoutId);

			ClaimsPrincipal user = HttpContext.User;
			if (user?.Identity.IsAuthenticated == true) {
				// delete local authentication cookie
				await HttpContext.SignOutAsync(Authentication.Scheme);

				// raise the logout event
				// await this.events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetDisplayName()));
			}

			// check if we need to trigger sign-out at an upstream identity provider
			if (vm.TriggerExternalSignout) {
				// build a return URL so the upstream provider will redirect back
				// to us after the user has logged out. this allows us to then
				// complete our single sign-out processing.
				string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

				// this triggers a redirect to the external provider for sign-out
				return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
			}

			// remove refresh token(s) for this user for this client
			try {
				await this._refreshTokenService.RemoveRefreshTokensAsync(user.GetSubjectId(), vm.ClientName);
			} catch (Exception ex) {
				Logger.Error(this, "Failed to remove refresh tokens", ex);
			}

			if (string.IsNullOrEmpty(vm.PostLogoutRedirectUri)) {
				// return View("LoggedOut", vm);
				return RedirectToAction("Index", "Home");
			} else {
				return Redirect(vm.PostLogoutRedirectUri);
			}
		}

	}

}