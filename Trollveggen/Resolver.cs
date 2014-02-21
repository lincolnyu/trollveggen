using System;
using System.Collections.Generic;
using System.Linq;

namespace Trollveggen
{
    /// <summary>
    ///  The class that keeps the objects user has registered by their types and keys user has specified
    /// </summary>
    public static class Resolver
    {
        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<object, object>> Registry
            = new Dictionary<Type, Dictionary<object, object>>();

        #endregion

        #region Methods

        /// <summary>
        ///  Returns an object that of the specified type (mostly like an interface to support);
        ///  If multiple objects exist, the one without a key is returned if existent or a random one is returned
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <returns>The object</returns>
        public static T Resolve<T>()
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
                {
                    return default(T);
                }

                var registeredTs = Registry[typeof(T)];
                if (registeredTs.ContainsKey(null))
                {
                    return (T)registeredTs[null];
                }

                if (registeredTs.Count == 0)
                {
                    Registry.Remove(typeof(T));
                    return default(T);
                }

                return (T)registeredTs.First().Value;
            }
        }

        /// <summary>
        ///  Returns the object of the specified type and with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        /// <returns>The object</returns>
        public static T Resolve<T>(object key)
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
                {
                    return default(T);
                }

                var registeredTs = Registry[typeof(T)];
                if (!registeredTs.ContainsKey(key))
                {
                    return default(T);
                }

                var r = registeredTs[key];
                return (T)r;
            }
        }

        /// <summary>
        ///  Registers the object without key (null key)
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        public static void Register<T>(T obj)
        {
            Register(obj, null);
        }

        /// <summary>
        ///  Registers the object with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="key">The key associated with the object</param>
        public static void Register<T>(T obj, object key)
        {
            lock (Registry)
            {
                Registry[typeof(T)][key] = obj;
            }
        }

        /// <summary>
        ///  Unregisters the object of the specified type without a key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public static void Release<T>()
        {
            Release<T>(null);
        }

        /// <summary>
        ///  Unregisters all objects of the specified type
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public static void ReleaseAll<T>()
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
                {
                    return;
                }
                Registry[typeof(T)].Clear();
                Registry.Remove(typeof (T));
            }
        }

        /// <summary>
        ///  Unregisters the object of the specified type and with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        public static void Release<T>(object key)
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof (T)))
                {
                    return;
                }
                Registry[typeof(T)].Remove(key);
                if (Registry[typeof(T)].Count == 0)
                {
                    Registry.Remove(typeof(T));
                }
            }
        }

        #endregion
    }
}
