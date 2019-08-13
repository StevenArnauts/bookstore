using System;

namespace Utilities {

	public static class Throw<TException> where TException : Exception {

		public static void When(bool condition, string message) {
			if (condition) {
				Exception result = (Exception)Activator.CreateInstance(typeof(TException), message);
				if (result == null) {
					throw new Exception("Exception type " + typeof(TException).FullName + " does not implement a constructor accepting a message argument");
				}
				throw result;
			}
		}	

	}

}
