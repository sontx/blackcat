using System;

namespace Blackcat.IoC
{
    /// <summary>
    /// Marks a class as a component which will be registered to IoC container automatically.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public bool Singleton { get; set; } = true;
    }

    /// <summary>
    /// Likes <see cref="ComponentAttribute"/> but it's more readable and appropriate.
    /// Should be annotated in service layer.
    /// </summary>
    public class ServiceAttribute : ComponentAttribute { }

    /// <summary>
    /// Likes <see cref="ComponentAttribute"/> but it's more readable and appropriate.
    /// Should be annotated in control layer (ex: main entry point).
    /// </summary>
    public class ControllerAttribute : ComponentAttribute { }

    /// <summary>
    /// Likes <see cref="ComponentAttribute"/> but it's more readable and appropriate.
    /// Should be annotated in persistence layer.
    /// </summary>
    public class RepositoryAttribute : ComponentAttribute { }
}