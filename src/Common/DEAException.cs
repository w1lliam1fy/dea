using System;

namespace DEA.Common
{
    /// <summary>
    /// Custom exception used to be caught by the error handler.
    /// </summary>
    class DEAException : Exception
    {
        /// <summary>
        /// Throws an exception intended to be caught by the error handler.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public DEAException(string message) : base(message) { }
    }
}
