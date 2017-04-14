using System;

namespace DEA.Common
{
    class DEAException : Exception
    {
        public DEAException(string message) : base(message) { }
    }
}
