namespace Gu.ChangeTracking.Tests.EqualByTests.PropertyValues
{
    using Gu.ChangeTracking.Tests.EqualByTests;

    public class Throws : ThrowsTests
    {
        public override bool EqualByMethod<T>(T x, T y)
        {
            return EqualBy.PropertyValues(x, y);
        }
    }
}
