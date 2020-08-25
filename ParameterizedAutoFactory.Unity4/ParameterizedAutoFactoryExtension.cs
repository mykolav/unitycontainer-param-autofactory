#if UNITY4_0_1
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
#elif UNITY5_X
using Unity;
using Unity.Builder;
using Unity.Extension;

#endif

namespace ParameterizedAutoFactory.Unity
{
    /// <summary>
    /// This is a unity extension class.
    /// If added to the container it hooks up
    /// the code that builds parameterized auto-factories
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
        protected override void Initialize()
        {
            Context.Strategies.Add(
                new ParameterizedAutoFactoryBuilderStrategy(Context.Container), 
                UnityBuildStage.PreCreation);
        }
    }
}