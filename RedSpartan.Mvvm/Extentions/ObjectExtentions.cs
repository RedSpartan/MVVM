using System;
using System.Collections.Generic;
using System.Text;

namespace RedSpartan
{
    public static class ObjectExtentions
    {
        public static bool Implements<T>(this object obj)
        {
            return (typeof(T).IsAssignableFrom(obj.GetType()));
        }

    }
}
