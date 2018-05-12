using Unity;
using Unity.Builder;
using Unity.Extension;

namespace ParameterizedAutoFactory
{
    public class UnityParameterizedAutoFactoryExtension : UnityContainerExtension
    {
        private readonly UnityContainer _container;

        public UnityParameterizedAutoFactoryExtension(UnityContainer container)
        {
            _container = container;
        }

        protected override void Initialize()
        {
            Context.Strategies.Add(
                new ParameterizedAutoFactoryBuilderStrategy(_container), 
                UnityBuildStage.PreCreation);
        }
    }
}