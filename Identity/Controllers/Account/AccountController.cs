using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bookstore.Identity.Entities;
using Bookstore.Identity.Models;
using Bookstore.Identity.Services;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
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

namespace Bookstore.Identity.Controllers {

	public class ProfileModel {

		public ProfileModel() {
			this.Claims = new List<Claim>();
		}

		public List<Claim> Claims { get; set; }

	}

	public class AccountController: Controller {

		private readonly UserRepository _userRepository;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IRefreshTokenStore _refreshTokenService;
		private readonly DoccleClient _doccle;
		private readonly IEventService _events;
		private readonly AccountService _account;

		public AccountController(UserRepository userRepository, IIdentityServerInteractionService interaction, IHttpContextAccessor httpContextAccessor, IClientStore clientStore, IAuthenticationSchemeProvider schemeProvider, IRefreshTokenStore refreshTokenService, DoccleClient doccle, IEventService events) {
			this._userRepository = userRepository;
			this._interaction = interaction;
			this._httpContextAccessor = httpContextAccessor;
			this._refreshTokenService = refreshTokenService;
			this._doccle = doccle;
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

			if (vm.IsExternalLoginOnly) {
				// we only have one option for logging in and it's an external provider
				return ExternalLogin(vm.ExternalLoginScheme, returnUrl);
			}

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
			var principal = this.CreatePrincipal(user);
			this._doccle.CreateAccount(user.Id, user.Name);
			await HttpContext.SignInAsync(Authentication.Scheme, principal);
			if(!string.IsNullOrEmpty(model.ReturnUrl)) {
				return Redirect(model.ReturnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Initiate roundtrip to external authentication provider
		/// </summary>
		[HttpGet]
		public IActionResult ExternalLogin(string provider, string returnUrl) {
			AuthenticationProperties props = new AuthenticationProperties {
				RedirectUri = Url.Action("ExternalLoginCallback"),
				Items = {
					{ "returnUrl", returnUrl }
				}
			};

			// start challenge and roundtrip the return URL
			props.Items.Add("scheme", provider);
			return Challenge(props, provider);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("itsme-callback")]
		public void ItsmeCallback([FromBody] AuthorizationResponse response) {
			// TODO remove this
			this.ExternalLoginCallback().Wait();
		}

		/// <summary>
		/// Post processing of external authentication
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> ExternalLoginCallback() {

			// read external identity from the temporary cookie
			AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
			if (result?.Succeeded != true) {
				throw new Exception("External authentication error");
			}

			// retrieve claims of the external user
			ClaimsPrincipal externalUser = result.Principal;
			List<Claim> claims = externalUser.Claims.ToList();

			// try to determine the unique id of the external user (issued by the provider)
			// the most common claim type for that are the sub claim and the NameIdentifier
			// depending on the external provider, some other claim type might be used
			Claim userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
			if (userIdClaim == null) {
				userIdClaim = claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
			}
			if (userIdClaim == null) {
				throw new Exception("Unknown userid");
			}

			// remove the user id claim from the claims collection and move to the userId property
			// also set the name of the external authentication provider
			claims.Remove(userIdClaim);
			string provider = result.Properties.Items["scheme"];
			string userId = userIdClaim.Value;

			// this is where custom logic would most likely be needed to match your users from the
			// external provider's authentication result, and provision the user as you see fit.
			// 
			// check if the external user is already provisioned
			User user = this._userRepository.Get(userId); // , provider);
			if (user == null) {
				// this sample simply auto-provisions new external user
				// another common approach is to start a registrations workflow first
				Logger.Warn(this, "User should be linked here");
				throw new Exception("User not found");
				// user = this.provisioningService.ProvisionUser(provider, userId, claims);
			} else {
				Logger.Warn(this, "Update claims here");
				// this.provisioningService.UpdateUser(user, claims);
			}

			List<Claim> additionalClaims = new List<Claim>();

			// if the external system sent a session id claim, copy it over
			// so we can use it for single sign-out
			Claim sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
			if (sid != null) {
				additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
			}

			// if the external provider issued an id_token, we'll keep it for signout
			AuthenticationProperties props = null;
			string idToken = result.Properties.GetTokenValue("id_token");
			if (idToken != null) {
				props = new AuthenticationProperties();
				props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
			}

			// issue authentication cookie for user
			await this._events.RaiseAsync(new UserLoginSuccessEvent(provider, userId, user.Email, user.Name));
			await HttpContext.SignInAsync(user.Id, user.Name, provider, props, additionalClaims.ToArray());

			// delete temporary cookie used during external authentication
			await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

			// validate return URL and redirect back to authorization endpoint or a local page
			string returnUrl = result.Properties.Items["returnUrl"];
			if (this._interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl)) {
				return Redirect(returnUrl);
			}

			return Redirect("~/");
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

			// return View("LoggedOut", vm);
			return Redirect(vm.PostLogoutRedirectUri);
		}
	}

}