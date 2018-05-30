using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Unity.ParameterizedAutoFactory.Core;

namespace ParameterizedAutoFactory.Unity4
{
    /// <summary>
    /// The <see cref="PreBuildUp" /> method of this class gets
    /// called while Unity is trying to resolve a dependency of
    /// the type's instance being created at that moment.
    /// </summary>
    internal class ParameterizedAutoFactoryBuilderStrategy : BuilderStrategy
    {
        private readonly UnityContainer _container;

        private readonly ParameterizedAutoFactoryProvider<
            IUnityContainer,
            ResolverOverride,
            ParameterByTypeOverride> _autoFactoryProvider;

        public ParameterizedAutoFactoryBuilderStrategy(UnityContainer container)
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

            if (_container.Registrations.Any(r => r.RegisteredType == type))
                return;

            if (!_autoFactoryProvider.TryGetOrCreate(type, out var autoFactory))
                return;

            context.Existing = autoFactory;
            context.BuildComplete = true;
        }
    }
}
