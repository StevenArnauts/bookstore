using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Utilities.Exceptions;
using Utilities.Logging;

namespace Utilities.REST {

	public class RestClient {

		private readonly HttpClient _httpClient;

		public RestClient(Uri serverUrl, HttpMessageHandler messageHandler = null) {
			HttpMessageHandler handler = messageHandler ?? new HttpClientHandler();
			this._httpClient = new HttpClient(handler) {
				BaseAddress = serverUrl
			};
			this._httpClient.DefaultRequestHeaders.Add("Accept-Language", "nl-be");
		}

		public void SetAccessToken(string accessToken) {
			if (this._httpClient.DefaultRequestHeaders.Contains("Authorization")) { this._httpClient.DefaultRequestHeaders.Remove("Authorization"); }
			this._httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
		}

		public string Get(Uri uri, CancellationToken cancellationToken) {
			Logger.Debug(this, "Requesting GET " + uri.AbsoluteUri + "...");
			return this.DoStringRequest(() => this._httpClient.GetAsync(uri, cancellationToken), cancellationToken);
		}

		public string Get(string path, CancellationToken cancellationToken) {
			Uri uri = new Uri(this._httpClient.BaseAddress, path);
			return this.Get(uri, cancellationToken);
		}

		public TResponse Get<TResponse>(string path, CancellationToken cancellationToken) {
			Uri uri = new Uri(this._httpClient.BaseAddress, path);
			return this.Get<TResponse>(uri, cancellationToken);
		}

		public TResponse Get<TResponse>(Uri uri, CancellationToken cancellationToken) {
			Logger.Debug(this, "Requesting GET " + uri.AbsoluteUri + "...");
			return this.Parse<TResponse>(() => this._httpClient.GetAsync(uri, cancellationToken), cancellationToken);
		}

		public byte[] Download(string path, CancellationToken cancellationToken) {
			Uri uri = new Uri(this._httpClient.BaseAddress, path);
			Logger.Debug(this, "Requesting GET " + uri.AbsoluteUri + "...");
			return this.DoByteRequest(() => this._httpClient.GetAsync(path, cancellationToken), cancellationToken);
		}

		public void Download(string url, string localPath, CancellationToken cancellationToken) {
			Task<Stream> downloadTask = this._httpClient.GetStreamAsync(url);
			using (FileStream stream = new FileStream(localPath, FileMode.Create, FileAccess.Write)) {
				downloadTask.Wait(cancellationToken);
				downloadTask.Result.CopyToAsync(stream, 4096, cancellationToken).Wait(cancellationToken);
				Logger.Debug(this, "Downloaded " + url + " to " + localPath);
			}
		}

		public TResponse Post<TRequest, TResponse>(string path, TRequest content, CancellationToken cancellationToken) {
			try {
				string requestJson = DefaultProtocol.Serializer.Serialize(content);
				Logger.Debug(this, "Posting " + requestJson + " to " + path + "...");
				HttpContent message = new StringContent(requestJson, DefaultProtocol.Encoding, "application/json");
				message.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				TResponse response = this.Parse<TResponse>(() => this._httpClient.PostAsync(path, message, cancellationToken), cancellationToken);
				return response;
			} catch (Exception ex) {
				Logger.Debug(this, ex.Message);
				throw;
			}
		}

		public void Post<TRequest>(string path, TRequest content, CancellationToken cancellationToken) {
			try {
				string requestJson = DefaultProtocol.Serializer.Serialize(content);
				Logger.Debug(this, "Posting " + requestJson + " to " + path + "...");
				HttpContent message = new StringContent(requestJson, DefaultProtocol.Encoding, "application/json");
				message.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				Task<HttpResponseMessage> post = this._httpClient.PostAsync(path, message, cancellationToken);
				post.Wait(cancellationToken);
			} catch (Exception ex) {
				Logger.Debug(this, ex.Message);
				throw;
			}
		}

		private TResponse Parse<TResponse>(Func<Task<HttpResponseMessage>> action, CancellationToken cancellationToken) {
			while (true) {
				string body = this.DoStringRequest(action, cancellationToken);
				TResponse result = string.IsNullOrEmpty(body) ? default(TResponse) : DefaultProtocol.Serializer.Deserialize<TResponse>(body);
				return result;
			}
		}

		private string DoStringRequest(Func<Task<HttpResponseMessage>> action, CancellationToken cancellationToken) {
			while (true) {
				Task<HttpResponseMessage> request = action.Invoke();
				HttpResponseMessage headers = this.GetHeaders(request, cancellationToken);
				if (headers == null) { continue; }
				Task<string> body = request.Result.Content.ReadAsStringAsync();
				body.Wait(cancellationToken);
				this.LogResponse(headers, body);
				return (body.Result);
			}
		}

		private byte[] DoByteRequest(Func<Task<HttpResponseMessage>> action, CancellationToken cancellationToken) {
			while (true) {
				Task<HttpResponseMessage> request = action.Invoke();
				HttpResponseMessage headers = this.GetHeaders(request, cancellationToken);
				if (headers == null) { continue; }
				Task<byte[]> body = request.Result.Content.ReadAsByteArrayAsync();
				body.Wait(cancellationToken);
				this.LogResponse(headers, body);
				return body.Result;
			}
		}

		private void LogResponse<TType>(HttpResponseMessage headers, Task<TType> body) {
			object result = body.Result;
			string resultText = result is byte[] ? ((byte[])result).Length.ToString(CultureInfo.InvariantCulture) : result.ToString();
			Logger.Debug(this, "Server responded " + (int)headers.StatusCode + ", date = " + (headers.Headers.Date == null ? "(not set)" : headers.Headers.Date.Value.ToString()) + ", etag = " + (headers.Headers.ETag == null ? "(not set)" : headers.Headers.ETag.Tag) + ", max-age = " + (headers.Headers.CacheControl != null && headers.Headers.CacheControl.MaxAge.HasValue ? headers.Headers.CacheControl.MaxAge.Value.ToString() : "(not set)") + ", body = " + resultText);
		}

		/// <summary>
		/// Executes the task to get the response headers. Should return them if all went to plan.
		/// If it returns null, the operation will be retried until a result is returned, or an exception is thrown.
		/// </summary>
		private HttpResponseMessage GetHeaders(Task<HttpResponseMessage> request, CancellationToken cancellationToken) {
			request.Wait(cancellationToken);
			if (!request.Result.IsSuccessStatusCode) {
				Logger.Debug(this, "Request failed: " + request.Result.StatusCode + "(" + request.Result.ReasonPhrase + ")");
				if (request.Result.StatusCode == HttpStatusCode.Unauthorized) {
					throw new UnauthorizedException();
				}
				Logger.Info("GetHeaders", request.Result.RequestMessage.RequestUri.ToString());
				throw new Exception("Server responded " + request.Result.StatusCode + "(" + request.Result.ReasonPhrase + ")"); // fail
			}
			return request.Result;
		}

		public void Dispose() {
			this._httpClient.Dispose();
		}

	}

}