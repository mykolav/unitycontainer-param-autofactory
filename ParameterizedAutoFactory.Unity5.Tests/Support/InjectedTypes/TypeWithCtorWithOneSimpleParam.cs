namespace ParameterizedAutoFactory.Unity5.Tests.Support.InjectedTypes
{
    public class TypeWithCtorWithOneSimpleParam
    {
        public TypeWithCtorWithOneSimpleParam(int param0)
        {
            Param0 = param0;
        }

        public int Param0 { get; }
    }
}