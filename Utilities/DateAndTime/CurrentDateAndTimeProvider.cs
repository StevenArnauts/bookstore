using System;

namespace Utilities.DateAndTime
{
    /// <summary>
    /// Returns the current date and time.
    /// </summary>
    public class CurrentDateAndTimeProvider : IDateAndTimeProvider
    {
        /// <inheritdoc />
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }

        /// <inheritdoc />
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}