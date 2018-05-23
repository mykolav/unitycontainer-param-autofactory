namespace ParameterizedAutoFactory.Unity4.Tests.Support.InjectedTypes
{
    public class TypeWithCtorWithTwoDependencyParams
    {
        public TypeWithCtorWithTwoDependencyParams(
            TypeWithParameterlessCtor typeWithParameterlessCtor,
            TypeWithCtorWithOneDependencyParam typeWithCtorWithOneDependencyParam)
        {
            TypeWithParameterlessCtor = typeWithParameterlessCtor;
            TypeWithCtorWithOneDependencyParam = typeWithCtorWithOneDependencyParam;
        }

        public TypeWithParameterlessCtor TypeWithParameterlessCtor { get; }
        public TypeWithCtorWithOneDependencyParam TypeWithCtorWithOneDependencyParam { get; }
    }
}