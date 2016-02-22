namespace Gu.ChangeTracking.Tests.CopyStubs
{
    public class WithArrayProperty
    {
        public WithArrayProperty()
        {
        }

        public WithArrayProperty(string name, int value)
        {
            this.Name = name;
            this.Value = value;
        }

        public WithArrayProperty(string name, int value, int[] array)
        {
            this.Name = name;
            this.Value = value;
            this.Array = array;
        }

        public string Name { get; set; }

        public int Value { get; set; }

        public int[] Array { get; set; }
    }
}