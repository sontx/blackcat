using System;

namespace Blackcat.Configuration
{
    public interface IDataAdapter
    {
        string ToString(object data);

        T ToObject<T>(object obj) where T : class;

        object ToObject(object obj, Type convertTo);
    }
}