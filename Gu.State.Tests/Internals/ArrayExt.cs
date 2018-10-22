namespace Gu.State.Tests.Internals
{
    using NUnit.Framework;

    public class ArrayExt
    {
        [Test]
        public void IndicesInDimensions()
        {
            var array = new int[2, 3];
            CollectionAssert.AreEqual(new[] { 0, 1 }, array.Indices(0));
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, array.Indices(1));
        }

        [Test]
        public void Indices()
        {
            var array = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            var expected = new[]
            {
                new[] { 0, 0 },
                new[] { 0, 1 },
                new[] { 0, 2 },
                new[] { 1, 0 },
                new[] { 1, 1 },
                new[] { 1, 2 },
            };
            CollectionAssert.AreEqual(expected, array.Indices());
            var arrayEnumerator = array.GetEnumerator();
            var indexEnumerator = expected.GetEnumerator();
            while (arrayEnumerator.MoveNext() && indexEnumerator.MoveNext())
            {
                Assert.AreEqual(arrayEnumerator.Current, array.GetValue((int[])indexEnumerator.Current));
            }
        }
    }
}
