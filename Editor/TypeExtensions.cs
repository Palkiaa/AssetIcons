using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor
{
    public static class TypeExtensions
    {
        //https://stackoverflow.com/a/1008185
        public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        {
            var attributeType = typeof(T);
            return type.GetCustomAttributes(attributeType, true)
              .Union(type.GetInterfaces().SelectMany(interfaceType =>
                  interfaceType.GetCustomAttributes(attributeType, true)))
              .Cast<T>();
        }
    }
}