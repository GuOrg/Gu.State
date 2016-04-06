namespace Gu.State.Tests.Internals.Refelection
{
    public static class TypeExtTypes
    {
        public class ComplexType
        {
            public int value;

            public int Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }
    }
}
