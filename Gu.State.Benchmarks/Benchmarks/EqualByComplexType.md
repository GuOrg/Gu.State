``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410073 Hz, Resolution=293.2489 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |     StdDev |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|-----------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.138 ns |  0.1053 ns |  0.0985 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    11.550 ns |  0.2473 ns |  0.2192 ns |   1.04 |    0.02 |           - |           - |           - |                   - |
|                              Func |     2.565 ns |  0.0049 ns |  0.0041 ns |   0.23 |    0.00 |           - |           - |           - |                   - |
|                          Comparer |     8.884 ns |  0.2083 ns |  0.2851 ns |   0.80 |    0.03 |           - |           - |           - |                   - |
|   EqualByPropertyValuesStructural | 2,799.217 ns | 54.5877 ns | 64.9827 ns | 250.92 |    5.57 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|   EqualByPropertyValuesReferences |   213.451 ns |  0.3746 ns |  0.3128 ns |  19.18 |    0.18 |      0.0367 |           - |           - |               232 B |
| EqualByPropertyValuesWithComparer |   918.545 ns | 17.8207 ns | 19.8076 ns |  82.74 |    2.02 |      0.1211 |      0.0248 |      0.0010 |               765 B |
|      EqualByFieldValuesStructural | 2,767.122 ns |  8.6142 ns |  7.1932 ns | 248.65 |    2.16 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|      EqualByFieldValuesReferences |   215.333 ns |  4.2016 ns |  4.1265 ns |  19.34 |    0.34 |      0.0367 |           - |           - |               232 B |
|    EqualByFieldValuesWithComparer |   994.329 ns | 19.5878 ns | 33.2616 ns |  89.00 |    2.53 |      0.1202 |      0.0191 |      0.0019 |               765 B |
