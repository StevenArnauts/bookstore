using System;
using System.Net;

namespace Utilities.Logging
{
	public static class ErrorConverter
	{
		public static Error Convert(Exception exception)
		{
			switch (exception)
			{
				case InvalidOperationException _:
					return CreateError(HttpStatusCode.BadRequest, exception.Message);
				case NotSupportedException _:
					return CreateError(HttpStatusCode.NotImplemented, exception.Message);
			}
			return CreateError(HttpStatusCode.InternalServerError, exception.Message);
		}

		/// <summary>
		///     Creates a standard Bringme Error response object.
		/// </summary>
		/// <param name="code">The HTTP status code.</param>
		/// <param name="message">The message.</param>
		/// <returns>Error object.</returns>
		private static Error CreateError(HttpStatusCode code, string message)
		{
			return new Error((int) code, message);
		}
	}
}