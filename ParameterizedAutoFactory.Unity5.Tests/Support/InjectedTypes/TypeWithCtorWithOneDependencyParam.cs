namespace ParameterizedAutoFactory.Unity5.Tests.Support.InjectedTypes
{
    public class TypeWithCtorWithOneDependencyParam
    {
        public TypeWithCtorWithOneDependencyParam(TypeWithParameterlessCtor typeWithParameterlessCtor)
        {
            TypeWithParameterlessCtor = typeWithParameterlessCtor;
        }

        public TypeWithParameterlessCtor TypeWithParameterlessCtor { get; }
    }
}