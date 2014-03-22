using System;
using System.Collections.Generic;

namespace Trollveggen
{
    /// <summary>
    ///  The class instance of which keeps the objects user has registered by their types and keys user has specified
    /// </summary>
    public class LocalCustomFactory<TKey>
    {
        #region Delegates

        public delegate object MatchDelegate(IDictionary<TKey, object> dictionary, TKey key);
        
        #endregion

        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        protected readonly Dictionary<Type, Dictionary<TKey, object>> Registry
            = new Dictionary<Type, Dictionary<TKey, object>>();

        #endregion

        #region Methods

        /// <summary>
        ///  Returns the object of the specified type and with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        /// <returns>The object</returns>
        public T Resolve<T>(TKey key)
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
        ///  Returns the object of the specified type and with the specified key 
        ///  using the specified re-attempt method to match the key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        /// <param name="match">The re-attempt method</param>
        /// <returns>The object</returns>
        public T Resolve<T>(TKey key, MatchDelegate match)
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
                {
                    return default(T);
                }

                var registeredTs = Registry[typeof(T)];

                var r = match(registeredTs, key);

                return (r != null) ? (T)r : default(T);
            }
        }

        /// <summary>
        ///  Registers the object with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="key">The key associated with the object</param>
        public void Register<T>(T obj, TKey key)
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
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
        public void Release<T>()
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
                {
                    return;
                }
                Registry[typeof(T)].Clear();
                Registry.Remove(typeof(T));
            }
        }

        /// <summary>
        ///  Unregisters the object of the specified type and with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        public void Release<T>(TKey key)
        {
            lock (Registry)
            {
                if (!Registry.ContainsKey(typeof(T)))
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
