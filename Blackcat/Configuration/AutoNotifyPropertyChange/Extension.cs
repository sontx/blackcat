using System;
using System.Collections;
using System.Collections.Generic;

namespace Blackcat.Configuration.AutoNotifyPropertyChange
{
    internal static class Extension
    {
        public static IEnumerable<T> OfType<T>(this IEnumerable en)
        {
            foreach (var el in en)
                if (el is T)
                    yield return (T)el;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> en, Func<T, bool> condition = null)
        {
            foreach (var el in en)
            {
                if (condition == null || condition(el))
                    return el;
            }
            return default;
        }

        public static T First<T>(this IEnumerable<T> en, Func<T, bool> condition = null)
        {
            foreach (var el in en)
            {
                if (condition == null || condition(el))
                    return el;
            }
            throw new InvalidOperationException();
        }

        public static bool Any(this IEnumerable en)
        {
#pragma warning disable 168
            foreach (var el in en)
#pragma warning restore 168
                return true;
            return false;
        }

        public delegate T Func<in TArg1, out T>(TArg1 arg);
    }
}

namespace System.Runtime.CompilerServices
{
    internal class ExtensionAttribute : Attribute
    {
    }
}