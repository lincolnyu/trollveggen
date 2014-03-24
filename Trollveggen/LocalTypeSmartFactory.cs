using System;
using Trollveggen.Helpers;

namespace Trollveggen
{
    public class LocalTypeSmartFactory : LocalCustomFactory<Type>
    {
        public new T Resolve<T>(Type key)
        {
            return Resolve<T>(key, (dictionary, type) => dictionary.GetCompatibleType(type));
        }
    }
}
