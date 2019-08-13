using System.Net.Http;

namespace Utilities {

	public abstract class ContentBase {

		public abstract HttpContent Content { get; }

	}

}