using System;
using Unity.ParameterizedAutoFactory.Core.Caches;

namespace Unity.ParameterizedAutoFactory.Core
{
    /// <summary>
    /// This class:
    ///   - builds a parameterized auto-factory if it is not in the cache.
    ///   - returns the appropriate instance from the cache otherwise.
    /// </summary>
    internal class ParameterizedAutoFactoryProvider<
        TUnityContainer,
        TResolverOverride,
        TParameterByTypeOverride>

        where TUnityContainer : class
        where TResolverOverride : class
        where TParameterByTypeOverride : class, TResolverOverride
    {
        private readonly object _existingAutoFactoriesLock;
        // Using ConditionalWeakTable<IUnityContainer, ICache<Type, object>> would've given us a cache per container.
        // As result we would've had a proper way of working with child containers.
        // But ConditionalWeakTable is only available from netstandard1.3 and onwards.
        // So we cannot use ConditionalWeakTable as:
        // Package Unity 4.0.1 is not compatible with netstandard1.3 (.NETStandard,Version=v1.3). 
        // Package Unity 4.0.1 supports:
        //  - net45 (.NETFramework,Version=v4.5)
        //  - portable-net45+win8+wp8+wpa81 (.NETPortable,Version=v0.0,Profile=Profile259)
        //  - win8 (Windows,Version=v8.0)
        //  - wp8 (WindowsPhone,Version=v8.0)
        private readonly ICache<Type, object> _existingAutoFactories;
        private readonly TUnityContainer _container;

        public ParameterizedAutoFactoryProvider(TUnityContainer container)
        {
            _existingAutoFactoriesLock = new object();
            _existingAutoFactories = new LeastRecentlyUsedCache<Type, object>();
            _container = container;
        }

        public bool TryGetOrCreate(
            Type typeOfAutoFactory, 
            Func<bool> isRegisteredInContainer,
            out object autoFactory)
        {
            autoFactory = null;

            // Likely, an overwhelming majority of types
            // that are being resolved, are not parameterized func-s.
            // So we check for that first to get out of the way
            // as fast as possible.
            if (!typeOfAutoFactory.IsParameterizedFunc())
                return false;

            // See if we can find an existing auto-factory before wasting time on locking.
            // If we find an existing auto-factory, that means we have decided to handle
            // the inspected type previously, and so we'll handle it now too.
            if (_existingAutoFactories.TryGetValue(typeOfAutoFactory, out autoFactory))
                return true;

            // If a type has been explicitly registered in the container,
            // the user expects that registration to be in effect,
            // so we must leave this type alone.
            if (isRegisteredInContainer())
                return false;

            lock (_existingAutoFactoriesLock)
            {
                // See if another thread created the auto-factory we are looking for,
                // while we were waiting to lock.
                if (_existingAutoFactories.TryGetValue(typeOfAutoFactory, out autoFactory))
                    return true;

                autoFactory = new ParameterizedAutoFactoryCreator<
                    TUnityContainer, 
                    TResolverOverride,
                    TParameterByTypeOverride>(_container, typeOfAutoFactory).Invoke();

                _existingAutoFactories.AddOrReplace(typeOfAutoFactory, autoFactory);

                return true;
            }
        }
    }
}
