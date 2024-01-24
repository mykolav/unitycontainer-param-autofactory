namespace ParameterizedAutoFactory.Tests.Support.InjectedTypes;

public class TypeWithCtorWithOneSimpleAndOneDependencyParam
{
    public TypeWithCtorWithOneSimpleAndOneDependencyParam(
        int param0,
        TypeWithCtorWithOneDependencyParam typeWithCtorWithOneDependencyParam)
    {
        Param0 = param0;
        TypeWithCtorWithOneDependencyParam = typeWithCtorWithOneDependencyParam;
    }

    public int Param0 { get; }
    public TypeWithCtorWithOneDependencyParam TypeWithCtorWithOneDependencyParam { get; }
}
