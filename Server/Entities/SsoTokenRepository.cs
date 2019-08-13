using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Bookstore.Entities {

	public class SsoTokenRepository : IRepository {

		private readonly List<SsoToken> _ssoTokens = new List<SsoToken>();

		public SsoToken Add(Receiver receiver) {
			string value = Guid.NewGuid().ToString("N").ToUpper();
			if (this._ssoTokens.Any(u => u.Value == value)) throw new Exception("SsoToken " + value + " already exists");
			SsoToken ssoToken = new SsoToken { Value = value, Receiver = receiver };
			this._ssoTokens.Add(ssoToken);
			return ssoToken;
		}

		public SsoToken Get(string value) {
			return this._ssoTokens.Get(u => u.Value == value);
		}
	
	}

}