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
        public string Key { get; set; }
        public string Description { get; set; }
    }
}