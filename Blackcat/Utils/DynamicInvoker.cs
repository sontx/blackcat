using System;
using System.Globalization;
using System.Reflection;

namespace Blackcat.Utils
{
    public static class DynamicInvoker
    {
        public static Type GetType(string namespaceName, string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemlby in assemblies)
            {
                if (assemlby.FullName.StartsWith(namespaceName))
                {
                    return assemlby.GetType($"{namespaceName}.{typeName}");
                }
            }
            return null;
        }

        public static Delegate AddEventHandler<T>(Type type, string eventName, T handler) where T : Delegate
        {
            var @event = type.GetEvent(eventName);
            Delegate convertedHandler = ConvertDelegate(handler, @event.EventHandlerType);
            @event.AddEventHandler(type, convertedHandler);
            return convertedHandler;
        }

        private static Delegate ConvertDelegate(Delegate originalDelegate, Type targetDelegateType)
        {
            return Delegate.CreateDelegate(
                targetDelegateType,
                originalDelegate.Target,
                originalDelegate.Method);
        }

        public static void RemoveEventHandler(Type type, string eventName, Delegate handler)
        {
            var @event = type.GetEvent(eventName);
            @event.RemoveEventHandler(type, handler);
        }

        public static object InvokeMethod(object instanceOrType, string methodName, params object[] parameters)
        {
            if (instanceOrType is Type)
            {
                var method = (instanceOrType as Type).GetMethod(methodName);
                if (method != null)
                {
                    var flags = BindingFlags.Static | BindingFlags.InvokeMethod;
                    return method.Invoke(null, flags, null, parameters, CultureInfo.CurrentCulture);
                }
            }
            else
            {
                var method = instanceOrType.GetType().GetMethod(methodName);
                if (method != null)
                {
                    var flags = BindingFlags.Instance | BindingFlags.InvokeMethod;
                    return method.Invoke(instanceOrType, flags, null, parameters, CultureInfo.CurrentCulture);
                }
            }
            throw new Exception($"Method {methodName} not found");
        }

        public static object GetPropertyValue(object instanceOrType, string propertyName)
        {
            if (instanceOrType is Type)
            {
                var property = (instanceOrType as Type).GetProperty(propertyName);
                if (property != null)
                {
                    var flags = BindingFlags.Static | BindingFlags.GetProperty;
                    return property.GetValue(null, flags, null, null, CultureInfo.CurrentCulture);
                }
            }
            else
            {
                var property = instanceOrType.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var flags = BindingFlags.Instance | BindingFlags.GetProperty;
                    return property.GetValue(instanceOrType, flags, null, null, CultureInfo.CurrentCulture);
                }
            }
            throw new Exception($"Property {propertyName} not found");
        }

        public static void SetPropertyValue(object instanceOrType, string propertyName, object value)
        {
            if (instanceOrType is Type)
            {
                var property = (instanceOrType as Type).GetProperty(propertyName);
                if (property != null)
                {
                    var flags = BindingFlags.Static | BindingFlags.SetProperty;
                    property.SetValue(null, value, flags, null, null, CultureInfo.CurrentCulture);
                }
            }
            else
            {
                var property = instanceOrType.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    var flags = BindingFlags.Instance | BindingFlags.SetProperty;
                    property.SetValue(instanceOrType, value, flags, null, null, CultureInfo.CurrentCulture);
                }
            }
            throw new Exception($"Property {propertyName} not found");
        }
    }
}