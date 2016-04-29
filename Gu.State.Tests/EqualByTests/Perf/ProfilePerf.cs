namespace Gu.State.Tests.EqualByTests.Perf
{
    using System;
    using System.Diagnostics;

    using NUnit.Framework;

    using static EqualByTypes;

    [Explicit("Debug perf")]
    public class ProfilePerf
    {
        private ComplexType x;
        private ComplexType y;
        private const int N = 1000000;

        [SetUp]
        public void SetUp()
        {
            this.x = new ComplexType { Name = "b", Value = 2 };
            this.y = new ComplexType { Name = "b", Value = 2 };

            EqualBy.PropertyValues(this.x, this.y);
            EqualBy.FieldValues(this.x, this.y);
        }

        [Test]
        public void PropertyValues()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < N; i++)
            {
                EqualBy.PropertyValues(this.x, this.y);
            }

            Console.WriteLine($"{N:N} PropertyValues took {sw.ElapsedMilliseconds} ms {(sw.Elapsed.TotalMilliseconds / N).ToString("F3")} ms/op");
        }

        [Test]
        public void FieldValues()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < N; i++)
            {
                EqualBy.FieldValues(this.x, this.y);
            }

            Console.WriteLine($"{N:N} FieldValues took {sw.ElapsedMilliseconds} ms {(sw.Elapsed.TotalMilliseconds / N).ToString("F3")} ms/op");
        }

        [Test]
        public void Comparer()
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < N; i++)
            {
                ComplexType.Comparer.Equals(this.x, this.y);
            }

            Console.WriteLine($"{N:N} Comparer took {sw.ElapsedMilliseconds} ms {(sw.Elapsed.TotalMilliseconds / N).ToString("F3")} ms/op");
        }
    }
}
