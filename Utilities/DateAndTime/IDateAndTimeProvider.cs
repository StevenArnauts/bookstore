using System;

namespace Utilities.DateAndTime
{
    public interface IDateAndTimeProvider
    {
        /// <summary>
        /// Gets a <see cref="T:System.DateTime"/> object that is set to the current date and time in the current time zone.
        /// </summary>
        DateTimeOffset Now();

        /// <summary>
        /// Gets a <see cref="T:System.DateTime"/> object that is set to the current date and time in UTC time zone.
        /// </summary>
        DateTimeOffset UtcNow();
    }
}