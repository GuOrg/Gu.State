namespace Gu.ChangeTracking.Tests.CopyStubs
{
    public class WithComplexProperty
    {
        public WithComplexProperty()
        {
        }

        public WithComplexProperty(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public int Value { get; set; }

        public ComplexType ComplexType { get; set; }
    }
}