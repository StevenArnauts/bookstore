using System;

namespace Utilities.Exceptions {

	public class BusinessException : ApplicationException {

		public BusinessException() {}
		public BusinessException(string message) : base(message) {}

	}

}