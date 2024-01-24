namespace ParameterizedAutoFactory.Tests.Support.InjectedTypes;

public class TypeWithCtorWithOneDependencyParam
{
    public TypeWithCtorWithOneDependencyParam(TypeWithParameterlessCtor typeWithParameterlessCtor)
    {
        TypeWithParameterlessCtor = typeWithParameterlessCtor;
    }

    public TypeWithParameterlessCtor TypeWithParameterlessCtor { get; }
}
