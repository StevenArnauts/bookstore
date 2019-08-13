using System.Net.Http;

namespace Utilities {

	public class NoContent : ContentBase
	{

		public override HttpContent Content => new StringContent("");

		public override string ToString()
		{
			return "";
		}

	}

}