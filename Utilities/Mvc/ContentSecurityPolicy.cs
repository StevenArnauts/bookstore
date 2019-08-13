using System.Collections.Generic;

namespace Utilities.Mvc {

	public sealed class ContentSecurityPolicyDirectiveBuilder {

		internal ContentSecurityPolicyDirectiveBuilder() { }

		internal List<string> Sources { get; set; } = new List<string>();

		public ContentSecurityPolicyDirectiveBuilder AllowSelf() {
			return this.Allow("'self'");
		}

		public ContentSecurityPolicyDirectiveBuilder AllowNone() {
			return this.Allow("none");
		}

		public ContentSecurityPolicyDirectiveBuilder AllowAny() {
			return this.Allow("*");
		}

		public ContentSecurityPolicyDirectiveBuilder AllowInline() {
			return this.Allow("'unsafe-inline'");
		}

		public ContentSecurityPolicyDirectiveBuilder Allow(string source) {
			this.Sources.Add(source);
			return this;
		}

	}

}