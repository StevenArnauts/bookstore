using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Utilities {

	public class JsonContent : ContentBase
	{

		private readonly string _content;

		public static JsonContent Empty = new JsonContent(null);

		public JsonContent(object content) {
			this._content = content == null ? "{}" : JsonConvert.SerializeObject(content, new JsonSerializerSettings
			{
				DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
			});
		}

		public override HttpContent Content  => new StringContent(this._content, Encoding.UTF8, "application/json");

		public override string ToString() {
			return this._content;
		}

	}

}