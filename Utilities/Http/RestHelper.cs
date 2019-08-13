using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utilities {

	public class RestHelper : HttpHelper {

		public RestHelper(string uri) : base(uri) { }

		public RestHelper(string uri, string path) : base(uri, path) { }

		public RestHelper(Uri uri) : base(uri) { }

		public async Task<T> Get<T>(string accessToken = null, HttpStatusCode expectedResult = HttpStatusCode.OK)
		{
			string json = await this.GetDocument(accessToken);
			T result = JsonConvert.DeserializeObject<T>(json);
			return result;
		}

		public async Task Post(object specificatiaon, string accessToken = null, HttpStatusCode expectedResult = HttpStatusCode.Created)
		{
			await this.Request(HttpMethod.Post, new JsonContent(specificatiaon), accessToken, HttpStatusCode.Created);
		}

	}

}