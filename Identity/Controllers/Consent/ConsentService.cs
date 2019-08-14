using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using ConsentModel = IdentityServer4.Models.Consent;

using Microsoft.Extensions.Logging;

namespace Bookstore.Identity.Controllers.Consent {

	public class ConsentService {

		private readonly IClientStore _clientStore;
		private readonly IResourceStore _resourceStore;
		private readonly IIdentityServerInteractionService _interaction;
		private readonly ILogger _logger;

		public ConsentService(IIdentityServerInteractionService interaction, IClientStore clientStore, IResourceStore resourceStore, ILogger logger) {
			this._interaction = interaction;
			this._clientStore = clientStore;
			this._resourceStore = resourceStore;
			this._logger = logger;
		}

		public async Task<IEnumerable<ConsentModel>> GetConsents() {
			IEnumerable<ConsentModel> consents = await this._interaction.GetAllUserConsentsAsync();
			return consents;
		}

		public async Task Revoke(string clientId) {
			await this._interaction.RevokeUserConsentAsync(clientId);
		}

		public async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model) {
			ProcessConsentResult result = new ProcessConsentResult();

			ConsentResponse grantedConsent = null;

			// user clicked 'no' - send back the standard 'access_denied' response
			if (model.Button == "no") {
				grantedConsent = ConsentResponse.Denied;
			}
			// user clicked 'yes' - validate the data
			else if (model.Button == "yes" && model != null) {
				// if the user consented to some scope, build the response model
				if (model.ScopesConsented != null && model.ScopesConsented.Any()) {
					System.Collections.Generic.IEnumerable<string> scopes = model.ScopesConsented;
					if (ConsentOptions.EnableOfflineAccess == false) {
						scopes = scopes.Where(x => x != IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
					}

					grantedConsent = new ConsentResponse {
						RememberConsent = model.RememberConsent,
						ScopesConsented = scopes.ToArray()
					};
				} else {
					result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
				}
			} else {
				result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
			}

			if (grantedConsent != null) {
				// validate return url is still valid
				AuthorizationRequest request = await this._interaction.GetAuthorizationContextAsync(model.ReturnUrl);
				if (request == null) {
					return result;
				}

				// communicate outcome of consent back to identityserver
				await this._interaction.GrantConsentAsync(request, grantedConsent);

				// indicate that's it ok to redirect back to authorization endpoint
				result.RedirectUri = model.ReturnUrl;
			} else {
				// we need to redisplay the consent UI
				result.ViewModel = await this.BuildViewModelAsync(model.ReturnUrl, model);
			}

			return result;
		}

		public async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl, ConsentInputModel model = null) {
			AuthorizationRequest request = await this._interaction.GetAuthorizationContextAsync(returnUrl);
			if (request != null) {
				Client client = await this._clientStore.FindEnabledClientByIdAsync(request.ClientId);
				if (client != null) {
					Resources resources = await this._resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
					if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any())) {
						return this.CreateConsentViewModel(model, returnUrl, request, client, resources);
					} else {
						this._logger.LogError("No scopes matching: {0}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y));
					}
				} else {
					this._logger.LogError("Invalid client id: {0}", request.ClientId);
				}
			} else {
				this._logger.LogError("No consent request matching request: {0}", returnUrl);
			}
			return null;
		}

		private ConsentViewModel CreateConsentViewModel(ConsentInputModel model, string returnUrl, AuthorizationRequest request, Client client, Resources resources) {
			ConsentViewModel vm = new ConsentViewModel {
				RememberConsent = model?.RememberConsent ?? true,
				ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
				ReturnUrl = returnUrl,
				ClientName = client.ClientName ?? client.ClientId,
				ClientUrl = client.ClientUri,
				ClientLogoUrl = client.LogoUri,
				AllowRememberConsent = client.AllowRememberConsent
			};

			vm.IdentityScopes = resources.IdentityResources.Select(x => this.CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();
			vm.ResourceScopes = resources.ApiResources.SelectMany(x => x.Scopes).Select(x => this.CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

			if (ConsentOptions.EnableOfflineAccess && resources.OfflineAccess) {
				vm.ResourceScopes = vm.ResourceScopes.Union(new ScopeViewModel[] {
					this.GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess) || model == null)
				});
			}

			return vm;
		}

		public ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check) {
			return new ScopeViewModel {
				Name = identity.Name,
				DisplayName = identity.DisplayName,
				Description = identity.Description,
				Emphasize = identity.Emphasize,
				Required = identity.Required,
				Checked = check || identity.Required
			};
		}

		public ScopeViewModel CreateScopeViewModel(Scope scope, bool check) {
			return new ScopeViewModel {
				Name = scope.Name,
				DisplayName = scope.DisplayName,
				Description = scope.Description,
				Emphasize = scope.Emphasize,
				Required = scope.Required,
				Checked = check || scope.Required
			};
		}

		private ScopeViewModel GetOfflineAccessScope(bool check) {
			return new ScopeViewModel {
				Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
				DisplayName = ConsentOptions.OfflineAccessDisplayName,
				Description = ConsentOptions.OfflineAccessDescription,
				Emphasize = true,
				Checked = check
			};
		}

	}

}