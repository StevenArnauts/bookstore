using System;

namespace Utilities.DateAndTime
{
    /// <summary>
    /// Static convenience class to get hold of the configured <see cref="IDateAndTimeProvider"/> without requiring dependency injection.
    /// </summary>
    public static class DateAndTimeFactory
    {
        public static IDateAndTimeProvider Provider { get; } = new CurrentDateAndTimeProvider();

        /// <summary>
        /// Gets a <see cref="T:System.DateTime"/> object that is set to the current date and time in UTC format.
        /// </summary>
        /// <returns>
        /// Current date and time.
        /// </returns>
        public static DateTimeOffset Now()
        {
            return Provider.Now();
        }

        /// <summary>
        /// Gets a <see cref="T:System.DateTime"/> object that is set to the current date in UTC format.
        /// </summary>
        /// <returns>
        /// Current date and time.
        /// </returns>
        public static DateTimeOffset UtcNow()
        {
            return Provider.UtcNow();
        }
    }
}