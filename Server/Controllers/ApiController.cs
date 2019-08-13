using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookstore.Entities;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Utilities.Logging;

namespace Bookstore.Controllers {

	/// <summary>
	/// Web API for senders
	/// </summary>
	[Route("api")]
	[ApiController]
	public class ApiController : ControllerBase {

		private readonly SenderRepository _senders;
		private readonly ReceiverRepository _receivers;
		private readonly AccountRepository _accounts;
		private readonly SsoTokenRepository _ssoTokens;

		public ApiController(SenderRepository senders, ReceiverRepository receivers, AccountRepository accounts, SsoTokenRepository ssoTokens) {
			this._senders = senders;
			this._receivers = receivers;
			this._accounts = accounts;
			this._ssoTokens = ssoTokens;
		}

		[HttpPost]
		[Route("senders/{senderId}/receivers")]
		public async Task<ActionResult<ReceiverRepresentation>> CreateReceiver([FromRoute] string senderId, [FromBody] ReceiverSpecification spec) {
			Logger.Info(this, "Creating receiver " + spec.Name + " for sender " + senderId);
			var sender = this._senders.GetById(senderId);
			var receiver = sender.Add(Guid.NewGuid().ToString("N").ToUpper(), spec.Name, spec.Secret);
			return this.Ok(ReceiverRepresentation.FromEntity(receiver));
		}

		[HttpPost]
		[Route("senders/{senderId}/receivers/{receiverId}/documents")]
		public async Task<ActionResult<DocumentRepresentation>> CreateDocument([FromRoute] string senderId, [FromRoute] string receiverId, [FromBody] DocumentSpecification spec) {
			Logger.Info(this, "Creating document " + spec.Title + " in receiver " + receiverId);
			var sender = this._senders.GetById(senderId);
			var receiver = sender.Receivers.Get(r => r.Id == receiverId);
			var doc = receiver.Add(spec.Id, spec.Title, spec.Timestamp);			
			return this.Ok(DocumentRepresentation.FromEntity(doc));
		}

		[HttpGet]
		[Route("receivers/{receiverId}/documents")]
		public async Task<ActionResult<IEnumerable<DocumentRepresentation>>> ListReceiver([FromRoute] string receiverId) {
			Logger.Info(this, "Listing documents in receiver " + receiverId);
			var receiver = this._receivers.GetById(receiverId);
			return this.Ok(receiver.Documents.Select(d => DocumentRepresentation.FromEntity(d)));
		}

		[HttpGet]
		[Route("users/{userId}/ebox")]
		public async Task<ActionResult<IEnumerable<DocumentRepresentation>>> ListEBox([FromRoute] string userId) {
			Logger.Info(this, "Listing eBox documents for user " + userId);
			// TODO: check token for user id and scope
			var account = this._accounts.GetByUserId(userId);
			if (account == null) return this.NotFound("User " + userId + " does not have an account");
			var receiver = account.Receivers.FirstOrDefault(r => r.Name == "ebox"); // could maybe also be more than one (in another version)
			return this.Ok(receiver?.Documents.Select(d => DocumentRepresentation.FromEntity(d)));
		}

		[HttpGet]
		[Route("users/{userId}/documents")]
		public async Task<ActionResult<IEnumerable<DocumentRepresentation>>> ListAllDocuments([FromRoute] string userId) {
			Logger.Info(this, "Listing all documents for user " + userId);
			var account = this._accounts.GetByUserId(userId);
			if (account == null) return this.NotFound("User " + userId + " does not have an account");
			return this.Ok(account.Receivers.SelectMany(r => r.Documents).Select(d => DocumentRepresentation.FromEntity(d)));
		}

		[HttpPost]
		[Route("receivers/{receiverId}/ssoTokens")]
		public async Task<ActionResult<SsoTokenRepresentation>> CreateSsoToken([FromRoute] string receiverId) {
			Logger.Info(this, "Creating sso token for receiver " + receiverId);
			var receiver = this._receivers.GetById(receiverId);
			var token = this._ssoTokens.Add(receiver);
			return this.Ok(new SsoTokenRepresentation {
				UserId = receiver.Account.UserId,
				Value = token.Value
			});
		}

		[HttpGet]
		[Route("ssoTokens/{value}/info")]
		public async Task<ActionResult<SsoTokenRepresentation>> GetSsoTokenInfo([FromRoute] string value) {
			Logger.Info(this, "Getting sso token info for " + value);
			var token = this._ssoTokens.Get(value);
			var account = token.Receiver.Account;			
			return this.Ok(new SsoTokenRepresentation {
				UserId = account.UserId
			});
		}

		[HttpPost]
		[Route("accounts")]
		public async Task<ActionResult<AccountRepresentation>> CreateAccount([FromBody] AccountSpecification spec) {
			Logger.Info(this, "Creating account" + spec.Name + " for " + spec.UserId);
			var account = this._accounts.Add(spec.Name, spec.UserId);
			return this.Ok(new AccountSpecification {
				UserId = account.UserId,
				Name = account.Name				
			});
		}

	}

}