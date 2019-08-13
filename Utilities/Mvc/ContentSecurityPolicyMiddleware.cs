using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Utilities.Mvc {

	public sealed class ContentSecurityPolicyMiddleware {

		private const string HEADER = "Content-Security-Policy";
		private readonly RequestDelegate next;
		private readonly ContentSecurityPolicyOptions options;

		public ContentSecurityPolicyMiddleware(RequestDelegate next, ContentSecurityPolicyOptions options) {
			this.next = next;
			this.options = options;
		}

		public async Task Invoke(HttpContext context) {
			context.Response.Headers.Remove(HEADER);
			context.Response.Headers.Remove("X-" + HEADER);
			context.Response.Headers.Add(HEADER, this.GetHeaderValue());
			context.Response.Headers.Add("X-" + HEADER, this.GetHeaderValue());
			await this.next(context);
		}

		private string GetHeaderValue() {
			string value = "";
			value += this.GetDirective("default-src", this.options.Defaults);
			value += this.GetDirective("script-src", this.options.Scripts);
			value += this.GetDirective("style-src", this.options.Styles);
			value += this.GetDirective("img-src", this.options.Images);
			value += this.GetDirective("font-src", this.options.Fonts);
			value += this.GetDirective("media-src", this.options.Media);
			value += this.GetDirective("connect-src", this.options.Connect);
			value += this.GetDirective("frame-src", this.options.Frames);
			return value;
		}

		private string GetDirective(string directive, List<string> sources) {
			return sources.Count > 0 ? $"{directive} {string.Join(" ", sources)}; " : "";
		}

	}

}