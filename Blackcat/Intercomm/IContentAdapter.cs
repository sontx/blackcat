using System;

namespace Blackcat.Intercomm
{
    public interface IContentAdapter
    {
        string ToString(object data);

        T ToObject<T>(string st);

        object ToObject(string st, Type objType);
    }
}