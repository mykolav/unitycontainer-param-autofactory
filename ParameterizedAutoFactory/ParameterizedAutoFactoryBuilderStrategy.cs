using System.Linq;
using Unity;
using Unity.Builder;
using Unity.Builder.Strategy;

namespace ParameterizedAutoFactory
{
    public class ParameterizedAutoFactoryBuilderStrategy : BuilderStrategy
    {
        private readonly UnityContainer _container;

        public ParameterizedAutoFactoryBuilderStrategy(UnityContainer container)
        {
            _container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = context.OriginalBuildKey.Type;

            if (_container.Registrations.Any(r => r.RegisteredType == type))
                return;

            //if (type.IsFunc())
            // ...

            //context.Existing = ...;
            context.BuildComplete = true;
        }
    }
}
