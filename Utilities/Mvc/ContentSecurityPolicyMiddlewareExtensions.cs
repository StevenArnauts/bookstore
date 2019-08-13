using System;

using Microsoft.AspNetCore.Builder;

namespace Utilities.Mvc {

	public static class ContentSecurityPolicyMiddlewareExtensions {

		public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder app, Action<ContentSecurityPolicyOptionsBuilder> builder) {
			ContentSecurityPolicyOptionsBuilder newBuilder = new ContentSecurityPolicyOptionsBuilder();
			builder(newBuilder);
			ContentSecurityPolicyOptions options = newBuilder.Build();
			return app.UseMiddleware<ContentSecurityPolicyMiddleware>(options);
		}

	}

}