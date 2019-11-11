using System;

namespace Blackcat.Configuration
{
    public class ConfigurationIOException : Exception
    {
        public ConfigurationIOException(string message) : base(message)
        {
        }

        public ConfigurationIOException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConfigurationIOException()
        {
        }
    }
}