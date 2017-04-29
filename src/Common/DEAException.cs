using System;

namespace DEA.Common
{
    class DEAException : Exception
    {
        /// <summary>
        /// Throws an exception intended to be caught by the error handler.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public DEAException(string message) : base(message) { }
    }
}
