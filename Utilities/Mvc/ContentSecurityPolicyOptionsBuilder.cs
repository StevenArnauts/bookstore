namespace Utilities.Mvc {

	public sealed class ContentSecurityPolicyOptionsBuilder {

		private readonly ContentSecurityPolicyOptions options = new ContentSecurityPolicyOptions();

		internal ContentSecurityPolicyOptionsBuilder() { }

		public ContentSecurityPolicyDirectiveBuilder Defaults { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Scripts { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Styles { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Images { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Fonts { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Media { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Connect { get; set; } = new ContentSecurityPolicyDirectiveBuilder();
		public ContentSecurityPolicyDirectiveBuilder Frames { get; set; } = new ContentSecurityPolicyDirectiveBuilder();

		internal ContentSecurityPolicyOptions Build() {
			this.options.Defaults = this.Defaults.Sources;
			this.options.Scripts = this.Scripts.Sources;
			this.options.Styles = this.Styles.Sources;
			this.options.Images = this.Images.Sources;
			this.options.Fonts = this.Fonts.Sources;
			this.options.Media = this.Media.Sources;
			this.options.Connect = this.Connect.Sources;
			this.options.Frames = this.Frames.Sources;
			return this.options;
		}

	}

}