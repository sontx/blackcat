using System;
using System.Globalization;
using System.Reflection;

namespace Blackcat.Types
{
    public sealed class MethodInvoker
    {
        private static readonly BindingFlags staticFlags = BindingFlags.Instance | BindingFlags.InvokeMethod;
        private static readonly BindingFlags instanceFlags = BindingFlags.Static | BindingFlags.InvokeMethod;

        private readonly object instanceOrType;
        private readonly MethodInfo method;
        private readonly bool isStatic;

        public MethodInvoker(object instanceOrType, string methodName, Type[] argumentTypes = null)
        {
            this.instanceOrType = instanceOrType;
            isStatic = instanceOrType is Type;
            if (argumentTypes == null)
            {
                method = isStatic
                ? ((Type)instanceOrType).GetMethod(methodName)
                : instanceOrType.GetType().GetMethod(methodName);
            }
            else
            {
                method = isStatic
                ? ((Type)instanceOrType).GetMethod(methodName, argumentTypes)
                : instanceOrType.GetType().GetMethod(methodName, argumentTypes);
            }
        }

        public object Invoke(params object[] parameters)
        {
            return isStatic
                ? method.Invoke(null, staticFlags, null, parameters, CultureInfo.CurrentCulture)
                : method.Invoke(instanceOrType, instanceFlags, null, parameters, CultureInfo.CurrentCulture);
        }
    }
}