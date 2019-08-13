using System.Collections.Generic;
using System.Net.Http;
using Utilities.Extensions;

namespace Utilities {

	public class FormContent : ContentBase
	{

		private readonly Dictionary<string, string> _values;

		public FormContent(Dictionary<string, string> values) {
			this._values = values;
		}

		public override HttpContent Content => new FormUrlEncodedContent(this._values);

		public override string ToString() {
			return this._values.Print("=", "&");
		}

	}

}