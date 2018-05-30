using System.Linq;
using Unity.ParameterizedAutoFactory.Core;
#if UNITY4_0_1
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using ParameterByTypeOverride=ParameterizedAutoFactory.Unity4.ParameterByTypeOverride;
#elif UNITY5_X
using Unity;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Resolution;
using ParameterByTypeOverride=ParameterizedAutoFactory.Unity5.ParameterByTypeOverride;
#endif

namespace ParameterizedAutoFactory.Unity
{
    /// <summary>
    /// The <see cref="PreBuildUp" /> method of this class gets
    /// called while Unity is trying to resolve a dependency of
    /// the type's instance being created at that moment.
    /// </summary>
    internal class ParameterizedAutoFactoryBuilderStrategy : BuilderStrategy
    {
        private readonly IUnityContainer _container;

        private readonly ParameterizedAutoFactoryProvider<
            IUnityContainer,
            ResolverOverride,
            ParameterByTypeOverride> _autoFactoryProvider;

        public ParameterizedAutoFactoryBuilderStrategy(IUnityContainer container)
        {
            _container = container;
            _autoFactoryProvider = new ParameterizedAutoFactoryProvider<
                IUnityContainer, 
                ResolverOverride, 
                ParameterByTypeOverride>(_container);
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = context.OriginalBuildKey.Type;

            if (_container.IsRegistered(type))
                return;

            if (!_autoFactoryProvider.TryGetOrCreate(type, out var autoFactory))
                return;

            context.Existing = autoFactory;
            context.BuildComplete = true;
        }
    }
}
