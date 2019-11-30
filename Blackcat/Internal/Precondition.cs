using System;

namespace Blackcat.Internal
{
    internal static class Precondition
    {
        public static void PropertyNotNull(object prop, string propName = "")
        {
            if (prop == null)
            {
                if (!string.IsNullOrEmpty(propName))
                    throw new PropertyNullException($"Property {propName} is null");
                throw new PropertyNullException();
            }
        }

        public static void ArgumentNotNull(object prop, string propName = "")
        {
            if (prop == null)
            {
                if (!string.IsNullOrEmpty(propName))
                    throw new ArgumentNullException($"Property {propName} is null");
                throw new ArgumentNullException();
            }
        }
    }
}