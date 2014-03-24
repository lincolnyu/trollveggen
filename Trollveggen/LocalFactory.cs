using System;
using System.Collections.Generic;
using System.Reflection;

namespace Trollveggen
{
    /// <summary>
    ///  The class instance of which keeps the objects user has registered by their types user has specified
    /// </summary>
    public class LocalFactory
    {
        #region Nested types

        /// <summary>
        ///  The collection that supports registration of multiple implementation object
        /// </summary>
        class MultiImplementors
        {
            #region Fields

            /// <summary>
            ///  The backing field for Objects
            ///  </summary>
            private LinkedList<object> _objects;

            /// <summary>
            ///  The backing field for ObjToLinkNode
            /// </summary>
            private Dictionary<object, LinkedListNode<object>> _objToLinkNode;

            #endregion

            #region Properties

            /// <summary>
            ///  The total number of objects
            /// </summary>
            public int Count
            {
                get
                {
                    return Objects.Count;
                }
            }

            /// <summary>
            ///  Bi-linked list of objects
            /// </summary>
            private LinkedList<object> Objects
            {
                get
                {
                    return _objects ?? (_objects = new LinkedList<object>());
                }
            }

            /// <summary>
            ///  Mapping from object the list node that contains it
            /// </summary>
            private Dictionary<object, LinkedListNode<object>> ObjToLinkNode
            {
                get
                {
                    return _objToLinkNode ?? (_objToLinkNode = new Dictionary<object, LinkedListNode<object>>());
                }
            }

            #endregion

            #region Methods

            /// <summary>
            ///  Returns the first object if any
            /// </summary>
            /// <returns>The first object or null</returns>
            public object GetFirstObject()
            {
                if (Count == 0)
                {
                    return null;
                }
                return Objects.First.Value;
            }

            /// <summary>
            ///  Adds a object to the tail of the collection
            /// </summary>
            /// <param name="obj">The object to add</param>
            public void AddObject(object obj)
            {
                Objects.AddLast(obj);
                ObjToLinkNode[obj] = Objects.Last;
            }

            /// <summary>
            ///  Retrieves the object that is registered after the specified one
            /// </summary>
            /// <param name="obj">The object the returned object should follow</param>
            /// <returns>The object or null</returns>
            public object GetObjectAfter(object obj)
            {
                if (!ObjToLinkNode.ContainsKey(obj))
                {
                    return null;
                }
                var node = ObjToLinkNode[obj];
                return node.Next != null ? node.Next.Value : null;
            }

            /// <summary>
            ///  Removes an object from the collection
            /// </summary>
            /// <param name="obj">The object to remove</param>
            public void Remove(object obj)
            {
                if (!ObjToLinkNode.ContainsKey(obj))
                {
                    return;
                }
                var node = ObjToLinkNode[obj];
                Objects.Remove(node);
                ObjToLinkNode.Remove(obj);
            }

            #endregion
        }

        #endregion

        #region Delegates

        /// <summary>
        ///  Delegates that registers an object with the specified type
        /// </summary>
        /// <param name="t">The type the object is registered with</param>
        /// <param name="obj">The object to register</param>
        private delegate void RegisterAction(Type t, object obj);

        #endregion

        #region Fields

        /// <summary>
        ///  The dictionary that keeps all the objects
        /// </summary>
        private readonly Dictionary<Type, object> _registry = new Dictionary<Type, object>();

        #endregion

        #region Methods

        /// <summary>
        ///  Returns an object that is of the specified type (mostly like an interface to support);
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <returns>The object</returns>
        public T Resolve<T>()
        {
            lock (_registry)
            {
                if (!_registry.ContainsKey(typeof(T)))
                {
                    return default(T);
                }

                var val = _registry[typeof(T)];
                var valAsDict = val as MultiImplementors;
                if (valAsDict != null)
                {
                    return (T)valAsDict.GetFirstObject();
                }
                return (T)val;
            }
        }

        /// <summary>
        ///  Returns an object that is of specified and is after the specified item in the list of same kind
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="after">The registered object that the returned object should follow</param>
        /// <returns>The object</returns>
        public T Resolve<T>(object after)
        {
            lock (_registry)
            {
                if (!_registry.ContainsKey(typeof(T)))
                {
                    return default(T);
                }
                var val = _registry[typeof(T)];
                var valAsCol = val as MultiImplementors;
                if (valAsCol != null)
                {
                    var o = valAsCol.GetObjectAfter(after);
                    return o != null? (T)o : default(T);
                }
                return default(T);
            }
        }

        /// <summary>
        ///  Registers the object; if an object is already registered with the type then it's replaced
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        public void Register<T>(T obj)
        {
            lock (_registry)
            {
                TreeRegister(obj, SingleRegister);
            }
        }


        /// <summary>
        ///  Register multiple item of the same declaring interface
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        /// <param name="obj">The object</param>
        public void MultiRegister<T>(T obj)
        {
            lock (_registry)
            {
                TreeRegister(obj, MultiRegister);
            }
        }

        /// <summary>
        ///  Registers an object with the the declared type and also all the interfaces the declared type implements
        /// </summary>
        /// <typeparam name="T">The delcared type the object is of, inherits or implements</typeparam>
        /// <param name="obj">The object</param>
        /// <param name="register">The register action used to register the object</param>
        private void TreeRegister<T>(T obj, RegisterAction register)
        {
            var t = typeof (T);
            var ti = t.GetTypeInfo();
            var intfs = ti.ImplementedInterfaces;
            register(t, obj);
            foreach (var intf in intfs)
            {
                register(intf, obj);
            }
        }

        /// <summary>
        ///  Single registers an object so a newer object will replace an older object registered with the same type
        /// </summary>
        /// <param name="t">The type that the object is registered with</param>
        /// <param name="obj">The object to register</param>
        private void SingleRegister(Type t, object obj)
        {
            _registry[t] = obj;
        }

        /// <summary>
        ///  Multple registers an object so a new object won't replace old one but be added
        /// </summary>
        /// <param name="t">The type that the object is registered with</param>
        /// <param name="obj">The object to register</param>
        private void MultiRegister(Type t, object obj)
        {
            if (_registry.ContainsKey(t))
            {
                var val = _registry[t];
                var valAsDict = val as MultiImplementors;
                if (valAsDict != null)
                {
                    valAsDict.AddObject(obj);
                }
                else
                {
                    var dict = new MultiImplementors();
                    dict.AddObject(val);
                    dict.AddObject(obj);
                    _registry[t] = dict;
                }
            }
            else
            {
                _registry[t] = obj;
            }
        }


        /// <summary>
        ///  Unregisters the object(s) of the specified type
        /// </summary>
        /// <typeparam name="T">The type the object is of or implements</typeparam>
        public void Release<T>()
        {
            lock (_registry)
            {
                _registry.Remove(typeof(T));
            }
        }

        /// <summary>
        ///  Release a specified object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Release<T>(T obj)
        {
            lock (_registry)
            {
                if (_registry.ContainsKey(typeof (T)))
                {
                    var val = _registry[typeof (T)];
                    var valAsDict = val as MultiImplementors;
                    if (valAsDict != null)
                    {
                        valAsDict.Remove(obj);
                        if (valAsDict.Count == 0)
                        {
                            _registry.Remove(typeof (T));
                        }
                    }
                    else
                    {
                        if (Equals(val, obj))
                        {
                            _registry.Remove(typeof(T));
                        }
                    }
                }
            }
        }

        #endregion
    }
}
