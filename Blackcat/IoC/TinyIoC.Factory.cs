using System;
using System.Reflection;

namespace Blackcat.IoC
{
    partial class TinyIoCContainer
    {
        #region Object Factories

        /// <summary>
        /// Provides custom lifetime management for ASP.Net per-request lifetimes etc.
        /// </summary>
        public interface ITinyIoCObjectLifetimeProvider
        {
            /// <summary>
            /// Gets the stored object if it exists, or null if not
            /// </summary>
            /// <returns>Object instance or null</returns>
            object GetObject();

            /// <summary>
            /// Store the object
            /// </summary>
            /// <param name="value">Object to store</param>
            void SetObject(object value);

            /// <summary>
            /// Release the object
            /// </summary>
            void ReleaseObject();
        }

        private abstract class ObjectFactoryBase
        {
            /// <summary>
            /// Whether to assume this factory sucessfully constructs its objects
            ///
            /// Generally set to true for delegate style factories as CanResolve cannot delve
            /// into the delegates they contain.
            /// </summary>
            public virtual bool AssumeConstruction { get { return false; } }

            /// <summary>
            /// The type the factory instantiates
            /// </summary>
            public abstract Type CreatesType { get; }

            /// <summary>
            /// Constructor to use, if specified
            /// </summary>
            public ConstructorInfo Constructor { get; protected set; }

            /// <summary>
            /// Create the type
            /// </summary>
            /// <param name="requestedType">Type user requested to be resolved</param>
            /// <param name="container">Container that requested the creation</param>
            /// <param name="parameters">Any user parameters passed</param>
            /// <param name="options"></param>
            /// <returns></returns>
            public abstract object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options);

            public virtual ObjectFactoryBase SingletonVariant
            {
                get
                {
                    throw new TinyIoCRegistrationException(GetType(), "singleton");
                }
            }

            public virtual ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    throw new TinyIoCRegistrationException(GetType(), "multi-instance");
                }
            }

            public virtual ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    throw new TinyIoCRegistrationException(GetType(), "strong reference");
                }
            }

            public virtual ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    throw new TinyIoCRegistrationException(GetType(), "weak reference");
                }
            }

            public virtual ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                throw new TinyIoCRegistrationException(GetType(), errorString);
            }

            public virtual void SetConstructor(ConstructorInfo constructor)
            {
                Constructor = constructor;
            }

            public virtual ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                return this;
            }
        }

        /// <summary>
        /// IObjectFactory that creates new instances of types for each resolution
        /// </summary>
        private class MultiInstanceFactory : ObjectFactoryBase
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            public override Type CreatesType { get { return registerImplementation; } }

            public MultiInstanceFactory(Type registerType, Type registerImplementation)
            {
                //#if NETFX_CORE
                //				if (registerImplementation.GetTypeInfo().IsAbstract() || registerImplementation.GetTypeInfo().IsInterface())
                //					throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");
                //#else
                if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");
                //#endif
                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                try
                {
                    return container.ConstructType(requestedType, registerImplementation, Constructor, parameters, options);
                }
                catch (TinyIoCResolutionException ex)
                {
                    throw new TinyIoCResolutionException(registerType, ex);
                }
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get
                {
                    return new SingletonFactory(registerType, registerImplementation);
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                return new CustomObjectLifetimeFactory(registerType, registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    return this;
                }
            }

            public override ObjectFactoryBase StrongReferenceVariant => this;
        }

        /// <summary>
        /// IObjectFactory that invokes a specified delegate to construct the object
        /// </summary>
        private class DelegateFactory : ObjectFactoryBase
        {
            private readonly Type registerType;

            private Func<TinyIoCContainer, NamedParameterOverloads, object> _factory;

            public override bool AssumeConstruction { get { return true; } }

            public override Type CreatesType { get { return registerType; } }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                try
                {
                    return _factory.Invoke(container, parameters);
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(registerType, ex);
                }
            }

            public DelegateFactory(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
            {
                if (factory == null)
                    throw new ArgumentNullException("factory");

                _factory = factory;

                this.registerType = registerType;
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return new WeakDelegateFactory(registerType, _factory);
                }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    return this;
                }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
            }
        }

        /// <summary>
        /// IObjectFactory that invokes a specified delegate to construct the object
        /// Holds the delegate using a weak reference
        /// </summary>
        private class WeakDelegateFactory : ObjectFactoryBase
        {
            private readonly Type registerType;

            private WeakReference _factory;

            public override bool AssumeConstruction { get { return true; } }

            public override Type CreatesType { get { return registerType; } }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                var factory = _factory.Target as Func<TinyIoCContainer, NamedParameterOverloads, object>;

                if (factory == null)
                    throw new TinyIoCWeakReferenceException(registerType);

                try
                {
                    return factory.Invoke(container, parameters);
                }
                catch (Exception ex)
                {
                    throw new TinyIoCResolutionException(registerType, ex);
                }
            }

            public WeakDelegateFactory(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory)
            {
                if (factory == null)
                    throw new ArgumentNullException("factory");

                _factory = new WeakReference(factory);

                this.registerType = registerType;
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    var factory = _factory.Target as Func<TinyIoCContainer, NamedParameterOverloads, object>;

                    if (factory == null)
                        throw new TinyIoCWeakReferenceException(registerType);

                    return new DelegateFactory(registerType, factory);
                }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return this;
                }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
            }
        }

        /// <summary>
        /// Stores an particular instance to return for a type
        /// </summary>
        private class InstanceFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private object _instance;

            public override bool AssumeConstruction { get { return true; } }

            public InstanceFactory(Type registerType, Type registerImplementation, object instance)
            {
                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "InstanceFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
                _instance = instance;
            }

            public override Type CreatesType
            {
                get { return registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                return _instance;
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get { return new MultiInstanceFactory(registerType, registerImplementation); }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return new WeakInstanceFactory(registerType, registerImplementation, _instance);
                }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    return this;
                }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
            }

            public void Dispose()
            {
                var disposable = _instance as IDisposable;

                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// Stores an particular instance to return for a type
        ///
        /// Stores the instance with a weak reference
        /// </summary>
        private class WeakInstanceFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private readonly WeakReference _instance;

            public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance)
            {
                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "WeakInstanceFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
                _instance = new WeakReference(instance);
            }

            public override Type CreatesType
            {
                get { return registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                var instance = _instance.Target;

                if (instance == null)
                    throw new TinyIoCWeakReferenceException(registerType);

                return instance;
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    return new MultiInstanceFactory(registerType, registerImplementation);
                }
            }

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return this;
                }
            }

            public override ObjectFactoryBase StrongReferenceVariant
            {
                get
                {
                    var instance = _instance.Target;

                    if (instance == null)
                        throw new TinyIoCWeakReferenceException(registerType);

                    return new InstanceFactory(registerType, registerImplementation, instance);
                }
            }

            public override void SetConstructor(ConstructorInfo constructor)
            {
                throw new TinyIoCConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
            }

            public void Dispose()
            {
                var disposable = _instance.Target as IDisposable;

                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// A factory that lazy instantiates a type and always returns the same instance
        /// </summary>
        private class SingletonFactory : ObjectFactoryBase, IDisposable
        {
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private readonly object SingletonLock = new object();
            private object _Current;

            public SingletonFactory(Type registerType, Type registerImplementation)
            {
                //#if NETFX_CORE
                //				if (registerImplementation.GetTypeInfo().IsAbstract() || registerImplementation.GetTypeInfo().IsInterface())
                //#else
                if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                    //#endif
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");

                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
            }

            public override Type CreatesType
            {
                get { return registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                if (parameters.Count != 0)
                    throw new ArgumentException("Cannot specify parameters for singleton types");

                lock (SingletonLock)
                    if (_Current == null)
                        _Current = container.ConstructType(requestedType, registerImplementation, Constructor, options);

                return _Current;
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get
                {
                    return this;
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                return new CustomObjectLifetimeFactory(registerType, registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    return new MultiInstanceFactory(registerType, registerImplementation);
                }
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                // We make sure that the singleton is constructed before the child container takes the factory.
                // Otherwise the results would vary depending on whether or not the parent container had resolved
                // the type before the child container does.
                GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }

            public override ObjectFactoryBase StrongReferenceVariant => this;

            public override ObjectFactoryBase WeakReferenceVariant
            {
                get
                {
                    return new WeakInstanceFactory(registerType, registerImplementation, _Current);
                }
            }

            public void Dispose()
            {
                if (_Current == null)
                    return;

                var disposable = _Current as IDisposable;

                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// A factory that offloads lifetime to an external lifetime provider
        /// </summary>
        private class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable
        {
            private readonly object SingletonLock = new object();
            private readonly Type registerType;
            private readonly Type registerImplementation;
            private readonly ITinyIoCObjectLifetimeProvider _LifetimeProvider;

            public CustomObjectLifetimeFactory(Type registerType, Type registerImplementation, ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorMessage)
            {
                if (lifetimeProvider == null)
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

                if (!IsValidAssignment(registerType, registerImplementation))
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");

                //#if NETFX_CORE
                //				if (registerImplementation.GetTypeInfo().IsAbstract() || registerImplementation.GetTypeInfo().IsInterface())
                //#else
                if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                    //#endif
                    throw new TinyIoCRegistrationTypeException(registerImplementation, errorMessage);

                this.registerType = registerType;
                this.registerImplementation = registerImplementation;
                _LifetimeProvider = lifetimeProvider;
            }

            public override Type CreatesType
            {
                get { return registerImplementation; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container, NamedParameterOverloads parameters, ResolveOptions options)
            {
                object current;

                lock (SingletonLock)
                {
                    current = _LifetimeProvider.GetObject();
                    if (current == null)
                    {
                        current = container.ConstructType(requestedType, registerImplementation, Constructor, options);
                        _LifetimeProvider.SetObject(current);
                    }
                }

                return current;
            }

            public override ObjectFactoryBase SingletonVariant
            {
                get
                {
                    _LifetimeProvider.ReleaseObject();
                    return new SingletonFactory(registerType, registerImplementation);
                }
            }

            public override ObjectFactoryBase MultiInstanceVariant
            {
                get
                {
                    _LifetimeProvider.ReleaseObject();
                    return new MultiInstanceFactory(registerType, registerImplementation);
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString)
            {
                _LifetimeProvider.ReleaseObject();
                return new CustomObjectLifetimeFactory(registerType, registerImplementation, lifetimeProvider, errorString);
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent, TinyIoCContainer child)
            {
                // We make sure that the singleton is constructed before the child container takes the factory.
                // Otherwise the results would vary depending on whether or not the parent container had resolved
                // the type before the child container does.
                GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }

            public void Dispose()
            {
                _LifetimeProvider.ReleaseObject();
            }
        }

        #endregion Object Factories
    }
}