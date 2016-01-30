namespace Gu.ChangeTracking.Tests.CopyStubs
{
    public class WithReadonlyField
    {
        public readonly int ReadonlyValue;
        public int Value;

        public WithReadonlyField(int readonlyValue, int value)
        {
            this.ReadonlyValue = readonlyValue;
            this.Value = value;
        }
    }
}