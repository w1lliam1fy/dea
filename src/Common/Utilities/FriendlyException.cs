using System;

namespace DEA.Common
{
    internal sealed class FriendlyException : Exception
    {
        public FriendlyException(string message) : base(message)
        {
        }
    }
}
