using System;
using Unity.ParameterizedAutoFactory.Core.Caches;

namespace Unity.ParameterizedAutoFactory.Core
{
    /// <summary>
    /// This class:
    ///   - builds a parameterized autofactory if it is not in the cache.
    ///   - returns the appropriate instance from the cache otherwise.
    /// </summary>
    public class ParameterizedAutoFactoryProvider<
        TUnityContainer,
        TResolverOverride,
        TParameterByTypeOverride>

        where TUnityContainer : class
        where TResolverOverride : class
        where TParameterByTypeOverride : class, TResolverOverride
    {
        private readonly object _existingAutoFactoriesLock;
        private readonly ICache<Type, object> _existingAutoFactories;
        private readonly TUnityContainer _container;

        public ParameterizedAutoFactoryProvider(TUnityContainer container)
        {
            _existingAutoFactoriesLock = new object();
            _existingAutoFactories = new LeastRecentlyUsedCache<Type, object>();
            _container = container;
        }

        public bool TryGetOrCreate(Type typeOfAutoFactory, out object autoFactory)
        {
            autoFactory = default(object);

            if (!typeOfAutoFactory.IsParameterizedFunc())
                return false;

            // See if we can find an existing auto-factory before wasting time on locking.
            if (_existingAutoFactories.TryGetValue(typeOfAutoFactory, out autoFactory))
                return true;

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
