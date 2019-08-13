namespace Utilities.Exceptions {

	/// <summary>
	/// To be used when some action on the domain is not allowed because of preconditions that are not met.
	/// </summary>
	public class OperationNotAllowedException : BusinessException {

		public OperationNotAllowedException(string message) : base(message) {}

	}

	public class ConfigurationException : BusinessException {

		public ConfigurationException(string message) : base(message) { }

	}

}