namespace Gu.State.Benchmarks
{
    using System;
    using System.Diagnostics;

    using BenchmarkDotNet.Running;

    public class Program
    {
        static void Main()
        {
            //BenchmarkRunner.Run<CopyArray2DBenchmarks>();
            //BenchmarkRunner.Run<CopyArray3DBenchmarks>();
            //BenchmarkRunner.Run<CopyListBenchmarks>();
            //BenchmarkRunner.Run<EqualByComplexTypeBenchmarks>();
            //BenchmarkRunner.Run<EqualByDynamicComplexTypeBenchmarks>();
            DoesNotLeakTrackedProperty();
            Console.ReadKey();
        }

        private static void DoesNotLeakTrackedProperty()
        {
            Console.WriteLine("Press any key to allocate.");
            Console.ReadKey();
            var x = new With<ComplexType> { ComplexType = new ComplexType("a", 1) };
            var y = new With<ComplexType> { ComplexType = new ComplexType("a", 1) };

            using (var tracker = Track.IsDirty(x, y))
            {
                var wrxc = new System.WeakReference(x.ComplexType);
                Console.WriteLine("before: {0}", wrxc.IsAlive);
                x.ComplexType = null;

                Console.WriteLine("Press any key to GC.Collect().");
                Console.ReadKey();
                GC.Collect();
                Console.WriteLine("after: {0}", wrxc.IsAlive);

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();

                //var wryc = new System.WeakReference(y.ComplexType);
                //y.ComplexType = null;
                //System.GC.Collect();
                //Assert.AreEqual(false, wryc.IsAlive);
            }
        }
    }
}
