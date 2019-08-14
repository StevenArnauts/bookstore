using Newtonsoft.Json;

namespace Bookstore.Identity.Controllers {
	public class AuthorizationResponse {
		public string Code { get; set; }
		public string Scope { get; set; }
		public string State { get; set; }
		[JsonProperty("session_state")]
		public string SessionState { get; set; }
	}

}