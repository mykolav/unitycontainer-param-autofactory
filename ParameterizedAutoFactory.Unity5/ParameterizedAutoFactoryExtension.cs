using Unity;
using Unity.Builder;
using Unity.Extension;

namespace ParameterizedAutoFactory.Unity5
{
    /// <summary>
    /// This is a unity extension class.
    /// If added to the container it hooks up
    /// the code that builds parameterized autofactories
    /// into Unity's dependency resolution pipeline.
    /// One way of adding this extension to the container is:
    /// <code>
    /// var container =
    ///     new UnityContainer()
    ///         .AddNewExtension{UnityParameterizedAutoFactoryExtension}();
    /// </code>
    /// </summary>
    public class UnityParameterizedAutoFactoryExtension : UnityContainerExtension
    {
        private readonly IUnityContainer _container;

        public UnityParameterizedAutoFactoryExtension(IUnityContainer container)
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