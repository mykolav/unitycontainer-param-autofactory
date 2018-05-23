using Microsoft.Practices.Unity;

namespace ParameterizedAutoFactory.Unity4.Tests.Support
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
                container.AddNewExtension<UnityParameterizedAutoFactoryExtension>();

            return container;
        }
    }
}