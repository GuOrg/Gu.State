namespace Gu.ChangeTracking.Tests.CopyStubs
{
    public class ComplexType
    {
        public ComplexType()
        {
        }

        public ComplexType(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public int Value { get; set; }
    }
}