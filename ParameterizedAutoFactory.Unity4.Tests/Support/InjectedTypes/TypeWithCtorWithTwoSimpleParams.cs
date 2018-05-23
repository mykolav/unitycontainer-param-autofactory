namespace ParameterizedAutoFactory.Unity4.Tests.Support.InjectedTypes
{
    public class TypeWithCtorWithTwoSimpleParams
    {
        public TypeWithCtorWithTwoSimpleParams(int param0, string param1)
        {
            Param0 = param0;
            Param1 = param1;
        }

        public int Param0 { get; }
        public string Param1 { get; }
    }
}