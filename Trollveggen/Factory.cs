using System;
using System.Collections.Generic;

namespace Trollveggen
{
    /// <summary>
    ///  The class that keeps the objects user has registered by their types user has specified
    /// </summary>
    public class Factory
    {
        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        private static readonly Dictionary<Type, object> Registry = new Dictionary<Type, object>();

        #endregion

        #region Methods

        /// <summary>
        ///  Returns an object that of the specified type (mostly like an interface to support);
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

                return (T)Registry[typeof (T)];
            }
        }

        /// <summary>
        ///  Registers the object; if an object is already registered with the type then it's replaced
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        public static void Register<T>(T obj)
        {
            lock (Registry)
            {
                Registry[typeof(T)] = obj;
            }
        }

        /// <summary>
        ///  Unregisters the object of the specified type without a key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public static void Release<T>()
        {
            lock (Registry)
            {
                Registry.Remove(typeof (T));
            }
        }

        #endregion
    }
}
