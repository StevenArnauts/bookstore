using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utilities.Logging;

namespace Utilities {

	public class HttpHelper {

		private readonly HttpClient _client;
		private readonly Uri _uri;

		public HttpHelper(string uri) : this(new Uri(uri)) { }

		public HttpHelper(string uri, string path) : this(new Uri(new Uri(uri), path)) { }

		public HttpHelper(Uri uri) {
			this._uri = uri;
			this._client = new HttpClient {
				BaseAddress = new Uri(this.Host)
			};
		}

		public string Host => $"{this._uri.Scheme}://{this._uri.Host}";

		public static string Base64Encode(string txt) {
			byte[] bytes = Encoding.UTF8.GetBytes(txt);
			return Convert.ToBase64String(bytes);
		}

		public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
			// Unix timestamp is seconds past epoch
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		public void ConfigureClient(Action<HttpClient> configurator) {
			configurator(this._client);
		}

		public async Task Delete(string accessToken = null) {
			await this.Request(HttpMethod.Delete, JsonContent.Empty, accessToken);
		}

		public async Task<string> GetDocument(string accessToken = null) {
			HttpResponseMessage response = await this.Request(HttpMethod.Get, new NoContent(), accessToken);
			string body = await response.Content.ReadAsStringAsync();
			Logger.Debug(this, "Response content = " + body);
			return body;
		}

		public async Task<Stream> GetStream(string accessToken = null) {
			HttpResponseMessage response = await this.Request(HttpMethod.Get, new NoContent(), accessToken);
			return await response.Content.ReadAsStreamAsync();
		}

		public async Task<HttpResponseMessage> PostForm(Dictionary<string, string> form) {
			return await this.Request(HttpMethod.Post, new FormContent(form));
		}

		protected async Task<HttpResponseMessage> Request(HttpMethod method, ContentBase body, string accessToken = null, HttpStatusCode expectedResult = HttpStatusCode.OK, string contentType = null) {
			Throw<Exception>.When(body == null, "Body is null");
			string action = this._uri.PathAndQuery;
			Logger.Debug(this, "Requesting " + method + " " + this._uri + " " + body + " ...");
			HttpRequestMessage request = new HttpRequestMessage(method, action) {
				Content = body.Content
			};
			if (!string.IsNullOrEmpty(accessToken)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			try {
				HttpResponseMessage response = await this._client.SendAsync(request);
				Logger.Debug(this, "Response status code = " + response.StatusCode + ", reason = " + response.ReasonPhrase);
				if (response.StatusCode == expectedResult) return response;
				Logger.Warn(this, "Server returned " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
				throw new Exception("Server returned unexpected status code " + response.StatusCode);
			} catch(Exception ex) {
				Logger.Error(this, ex);
				throw;
			}
			
		}

	}

}