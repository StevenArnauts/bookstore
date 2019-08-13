using System;

namespace Utilities.Extensions {

	public static class GuidExtensions {

		public static string Format(this Guid source) {
			return (source.ToString("N").ToUpper());
		}

		public static Guid Create(ulong source) {
			byte[] data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			byte[] bytes = BitConverter.GetBytes(source);
			Array.Reverse(bytes);
			Array.Copy(bytes, 0, data, 8, 8);			
			Guid id = new Guid(data);
			return id;
		}

	}

}
