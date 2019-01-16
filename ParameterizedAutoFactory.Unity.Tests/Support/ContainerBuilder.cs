using ParameterizedAutoFactory.Unity;
#if UNITY4_0_1
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Unity4;
#elif UNITY5_X
using ParameterizedAutoFactory.Unity5;
using Unity;
#endif

namespace ParameterizedAutoFactory.Tests.Support
{
    internal class ContainerBuilder
    {
        private bool _addParameterizedAutoFactoryExtension;
        public ContainerBuilder AddParameterizedAutoFactoryExtension()
        {
            _addParameterizedAutoFactoryExtension = true;
            return this;
        }

        public IUnityContainer Build()
        {
            var container = new UnityContainer();

            if (_addParameterizedAutoFactoryExtension)
                container.AddParameterizedAutoFactoryExtension();

            return container;
        }
    }

    internal static class UnityContainerExtensions
    {
        internal static IUnityContainer AddParameterizedAutoFactoryExtension(this IUnityContainer container)
            => container.AddNewExtension<UnityParameterizedAutoFactoryExtension>();
    }
}