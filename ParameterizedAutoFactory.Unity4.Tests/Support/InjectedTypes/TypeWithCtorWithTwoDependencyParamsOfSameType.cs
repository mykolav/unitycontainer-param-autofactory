namespace ParameterizedAutoFactory.Unity4.Tests.Support.InjectedTypes
{
    public class TypeWithCtorWithTwoDependencyParamsOfSameType
    {
        public TypeWithCtorWithTwoDependencyParamsOfSameType(
            TypeWithParameterlessCtor typeWithParameterlessCtor,
            TypeWithCtorWithOneDependencyParam typeWithCtorWithOneDependencyParam0,
            TypeWithCtorWithOneDependencyParam typeWithCtorWithOneDependencyParam1)
        {
            TypeWithParameterlessCtor = typeWithParameterlessCtor;
            TypeWithCtorWithOneDependencyParam0 = typeWithCtorWithOneDependencyParam0;
            TypeWithCtorWithOneDependencyParam1 = typeWithCtorWithOneDependencyParam1;
        }

        public TypeWithParameterlessCtor TypeWithParameterlessCtor { get; }
        public TypeWithCtorWithOneDependencyParam TypeWithCtorWithOneDependencyParam0 { get; }
        public TypeWithCtorWithOneDependencyParam TypeWithCtorWithOneDependencyParam1 { get; }
    }
}