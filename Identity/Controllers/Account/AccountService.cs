// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bookstore.Identity.Controllers {

	public class AccountService {

		private readonly IClientStore _clientStore;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAuthenticationSchemeProvider _schemeProvider;

		public AccountService(IIdentityServerInteractionService interaction, IHttpContextAccessor httpContextAccessor, IAuthenticationSchemeProvider schemeProvider, IClientStore clientStore) {
			this._interaction = interaction;
			this._httpContextAccessor = httpContextAccessor;
			this._schemeProvider = schemeProvider;
			this._clientStore = clientStore;
		}

		public async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl) {
			IdentityServer4.Models.AuthorizationRequest context = await this._interaction.GetAuthorizationContextAsync(returnUrl);
			if (context?.IdP != null) {
				// this is meant to short circuit the UI and only trigger the one external IdP
				return new LoginViewModel {
					EnableLocalLogin = false,
					ReturnUrl = returnUrl,
					Email = context?.LoginHint,
					ExternalProviders = new ExternalProvider[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
				};
			}

			System.Collections.Generic.IEnumerable<AuthenticationScheme> schemes = await this._schemeProvider.GetAllSchemesAsync();

			System.Collections.Generic.List<ExternalProvider> providers = schemes
				.Where(x => x.DisplayName != null ||
							(AccountOptions.WindowsAuthenticationEnabled &&
							 x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
				)
				.Select(x => new ExternalProvider {
					DisplayName = x.DisplayName,
					AuthenticationScheme = x.Name
				}).ToList();

			bool allowLocal = true;
			if (context?.ClientId != null) {
				IdentityServer4.Models.Client client = await this._clientStore.FindEnabledClientByIdAsync(context.ClientId);
				if (client != null) {
					allowLocal = client.EnableLocalLogin;

					if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any()) {
						providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
					}
				}
			}

			return new LoginViewModel {
				AllowRememberLogin = AccountOptions.AllowRememberLogin,
				EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
				ReturnUrl = returnUrl,
				Email = context?.LoginHint,
				ExternalProviders = providers.ToArray()
			};
		}

		public async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model) {
			LoginViewModel vm = await this.BuildLoginViewModelAsync(model.ReturnUrl);
			vm.Email = model.Email;
			vm.RememberLogin = model.RememberLogin;
			return vm;
		}

		public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId) {
			LogoutViewModel vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

			System.Security.Claims.ClaimsPrincipal user = this._httpContextAccessor.HttpContext.User;
			if (user?.Identity.IsAuthenticated != true) {
				// if the user is not authenticated, then just show logged out page
				vm.ShowLogoutPrompt = false;
				return vm;
			}

			IdentityServer4.Models.LogoutRequest context = await this._interaction.GetLogoutContextAsync(logoutId);
			if (context?.ShowSignoutPrompt == false) {
				// it's safe to automatically sign-out
				vm.ShowLogoutPrompt = false;
				return vm;
			}

			// show the logout prompt. this prevents attacks where the user
			// is automatically signed out by another malicious web page.
			return vm;
		}

		public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId) {
			// get context information (client name, post logout redirect URI and iframe for federated signout)
			IdentityServer4.Models.LogoutRequest logout = await this._interaction.GetLogoutContextAsync(logoutId);

			LoggedOutViewModel vm = new LoggedOutViewModel {
				AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
				PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
				ClientName = logout?.ClientId,
				SignOutIframeUrl = logout?.SignOutIFrameUrl,
				LogoutId = logoutId
			};

			System.Security.Claims.ClaimsPrincipal user = this._httpContextAccessor.HttpContext.User;
			if (user?.Identity.IsAuthenticated == true) {
				string idp = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
				if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider) {
					bool providerSupportsSignout = await this._httpContextAccessor.HttpContext.GetSchemeSupportsSignOutAsync(idp);
					if (providerSupportsSignout) {
						if (vm.LogoutId == null) {
							// if there's no current logout context, we need to create one
							// this captures necessary info from the current logged in user
							// before we signout and redirect away to the external IdP for signout
							vm.LogoutId = await this._interaction.CreateLogoutContextAsync();
						}

						vm.ExternalAuthenticationScheme = idp;
					}
				}
			}

			return vm;
		}
	}
}
