using System;
using System.Collections.Generic;

namespace Trollveggen
{
    /// <summary>
    ///  The class that keeps the objects user has registered by their types and keys user has specified
    /// </summary>
    public static class CustomFactory<TKey>
    {
        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<TKey, object>> Registry
            = new Dictionary<Type, Dictionary<TKey, object>>();

        #endregion

        #region Methods

        /// <summary>
        ///  Returns the object of the specified type and with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        /// <returns>The object</returns>
        public static T Resolve<T>(TKey key)
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
        ///  Registers the object with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="key">The key associated with the object</param>
        public static void Register<T>(T obj, TKey key)
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof (T)))
                {
                    Registry.Add(typeof(T), new Dictionary<TKey, object>());
                }
                Registry[typeof(T)][key] = obj;
            }
        }

        /// <summary>
        ///  Unregisters all objects of the specified type
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public static void Release<T>()
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
        public static void Release<T>(TKey key)
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
