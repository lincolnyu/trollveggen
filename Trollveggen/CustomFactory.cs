namespace Trollveggen
{
    /// <summary>
    ///  The factory that keeps the objects user has registered by their types and keys user has specified
    /// </summary>
    public static class CustomFactory<TKey>
    {
        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        private static readonly LocalCustomFactory<TKey> LocalFactory = new LocalCustomFactory<TKey>();

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
            return LocalFactory.Resolve<T>(key);
        }

        /// <summary>
        ///  Registers the object with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="key">The key associated with the object</param>
        public static void Register<T>(T obj, TKey key)
        {
            LocalFactory.Register(obj, key);
        }

        /// <summary>
        ///  Unregisters all objects of the specified type
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public static void Release<T>()
        {
            LocalFactory.Release<T>();
        }

        /// <summary>
        ///  Unregisters the object of the specified type and with the specified key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="key">The key associated with the object</param>
        public static void Release<T>(TKey key)
        {
            LocalFactory.Release<T>(key);
        }

        #endregion
    }
}
