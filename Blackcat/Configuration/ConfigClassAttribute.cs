using System;

namespace Blackcat.Configuration
{
    /// <inheritdoc />
    /// <summary>
    ///     Annotates a class is a configuration type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigClassAttribute : Attribute
    {
        /// <summary>
        ///     Creates <see cref="ConfigClassAttribute" />.
        /// </summary>
        public ConfigClassAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}