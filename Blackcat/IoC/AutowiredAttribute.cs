using System;

namespace Blackcat.IoC
{
    /// <summary>
    /// Auto inject missing services to properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {
    }
}