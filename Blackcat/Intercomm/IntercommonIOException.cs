using System;

namespace Blackcat.Intercomm
{
    public class IntercommonIOException : Exception
    {
        public IntercommonIOException(string message) : base(message)
        {
        }

        public IntercommonIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IntercommonIOException()
        {
        }
    }
}