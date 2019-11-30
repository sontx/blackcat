using System;

namespace Blackcat.Internal
{
    internal class PropertyNullException : Exception
    {
        public PropertyNullException(string message) : base(message)
        {
        }

        public PropertyNullException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PropertyNullException()
        {
        }
    }
}