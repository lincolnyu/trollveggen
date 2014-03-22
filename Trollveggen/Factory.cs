namespace Trollveggen
{
    /// <summary>
    ///  The factory that keeps the objects user has registered by their types user has specified
    /// </summary>
    public static class Factory
    {
        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        private static readonly LocalFactory LocalFactory = new LocalFactory();

        #endregion

        #region Methods

        /// <summary>
        ///  Returns an object that of the specified type (mostly like an interface to support);
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <returns>The object</returns>
        public static T Resolve<T>()
        {
            return LocalFactory.Resolve<T>();
        }

        /// <summary>
        ///  Returns an object that is of specified and is after the specified item in the list of same kind
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="after">The registered object that the returned object should follow</param>
        /// <returns>The object</returns>
        public static T Resolve<T>(object after)
        {
            return LocalFactory.Resolve<T>(after);
        }

        /// <summary>
        ///  Registers the object; if an object is already registered with the type then it's replaced
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        public static void Register<T>(T obj)
        {
            LocalFactory.Register(obj);
        }

        /// <summary>
        ///  Register multiple item of the same declaring interface
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        public static void MultiRegister<T>(T obj)
        {
            LocalFactory.MultiRegister(obj);
        }

        /// <summary>
        ///  Unregisters the object of the specified type without a key
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public static void Release<T>()
        {
            LocalFactory.Release<T>();
        }

        /// <summary>
        ///  Release a specified object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void Release<T>(T obj)
        {
            LocalFactory.Release(obj);
        }

        #endregion
    }
}
