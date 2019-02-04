using System.Linq;
using Unity.ParameterizedAutoFactory.Core;
#if UNITY4_0_1
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using ParameterByTypeOverride=ParameterizedAutoFactory.Unity4.ParameterByTypeOverride;
#elif UNITY5_X
using Unity;
using Unity.Resolution;
using Unity.Builder;
using Unity.Builder.Strategy;
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

        public ParameterizedAutoFactoryBuilderStrategy(IUnityContainer container = null)
        {
            _container = container;
            _autoFactoryProvider = new ParameterizedAutoFactoryProvider<
                IUnityContainer, 
                ResolverOverride, 
                ParameterByTypeOverride>();
        }

        public override void PreBuildUp(IBuilderContext context)
        {
#if UNITY4_0_1
            var strategy = GetOverridingStrategy(context);
            strategy.DoPreBuildUp(context);
#elif UNITY5_X
            DoPreBuildUp(context);
#endif
        }

        private void DoPreBuildUp(IBuilderContext context)
        {
            var container = GetContainer(context);
            var type = context.OriginalBuildKey.Type;

            const string withoutNameOrAnyName = "";
#if UNITY4_0_1
            var foundOrCreated = _autoFactoryProvider.TryBuildAutoFactory(
                typeOfAutoFactory: type,
                container: container,
                isRegisteredInContainer: () => container.IsRegistered(type, withoutNameOrAnyName),
                autoFactory: out var autoFactory);

            if (!foundOrCreated)
                return;
#elif UNITY5_X
            var foundOrCreated = _autoFactoryProvider.TryGetOrBuildAutoFactoryCreator(
                typeOfAutoFactory: type,
                isRegisteredInContainer: () => container.IsRegistered(type, withoutNameOrAnyName),
                createAutoFactory: out var createAutoFactory);

            if (!foundOrCreated)
                return;

            var autoFactory = createAutoFactory(container);
#endif

            context.Existing = autoFactory;
            context.BuildComplete = true;
        }

        private IUnityContainer GetContainer(IBuilderContext context)
        {
#if UNITY4_0_1
            return _container;
#elif UNITY5_X
            return context.Container;
#endif
        }

        private ParameterizedAutoFactoryBuilderStrategy GetOverridingStrategy(IBuilderContext context) 
            => context.Strategies.OfType<ParameterizedAutoFactoryBuilderStrategy>().Last();
    }
}
