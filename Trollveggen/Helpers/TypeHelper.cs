using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trollveggen.Helpers
{
    public static class TypeHelper
    {
        public static T GetCompatibleType<T>(this IDictionary<Type, T> typeDictionary, Type queryType)
        {
            if (typeDictionary.ContainsKey(queryType))
            {
                return typeDictionary[queryType];
            }

            var typeInfo = queryType.GetTypeInfo();
            Type type = null;
            if (typeInfo.IsClass)
            {
                type = queryType;
                do
                {
                    type = type.GetTypeInfo().BaseType;
                } while (type != null && typeDictionary.ContainsKey(type));
            }

            if (type != null) return default(T);

            // check interfaces
            var interfaces = typeInfo.ImplementedInterfaces;
            foreach (var intf in interfaces.Where(typeDictionary.ContainsKey))
            {
                type = intf;
                break;
            }

            return type == null ? default(T) : typeDictionary[type];
        }
    }
}
