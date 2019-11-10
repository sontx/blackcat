using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blackcat.IoC
{
    /// <summary>
    /// The name should be ApplicationContext/AppContext
    /// but it's duplicated with other classes in System and WinForm namespaces.
    /// </summary>
    public sealed class App32Context : IDisposable
    {
        private readonly TinyIoCContainer container;

        public App32Context(params Assembly[] assemblies)
        {
            container = new TinyIoCContainer();
            var result = container.AutoRegister(GetRegisterAssemblies(assemblies), DuplicateImplementationActions.Fail, HandleRegisterType);
            ApplyAttributeSettings(result);
        }

        private Assembly[] GetRegisterAssemblies(Assembly[] assemblies)
        {
            return assemblies == null || assemblies.Length == 0
                ? new Assembly[] { Assembly.GetEntryAssembly() }
                : assemblies;
        }

        private bool HandleRegisterType(Type registerType)
        {
            var attr = registerType.GetCustomAttribute<ComponentAttribute>();
            return attr != null;
        }

        private void ApplyAttributeSettings(Dictionary<Type, TinyIoCContainer.RegisterOptions> result)
        {
            foreach (var pair in result)
            {
                var type = pair.Key;
                var option = pair.Value;

                var attr = type.GetCustomAttribute<ComponentAttribute>();

                if (attr.Singleton)
                    option.AsSingleton();
                else
                    option.AsMultiInstance();
            }
        }

        public T Resolve<T>() where T : class
        {
            var instance = container.Resolve<T>(new ResolveOptions { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail });
            if (instance != null)
            {
                container.BuildUp(instance, new ResolveOptions { PropertyFilter = HandleAutowiredType, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail });
            }
            return instance;
        }

        private bool HandleAutowiredType(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<AutowiredAttribute>();
            return attr != null;
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}