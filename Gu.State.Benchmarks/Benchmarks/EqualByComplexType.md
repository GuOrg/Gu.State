``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410076 Hz, Resolution=293.2486 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |     StdDev |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|-----------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.804 ns |  0.0935 ns |  0.0875 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    12.035 ns |  0.2674 ns |  0.2501 ns |   1.02 |    0.03 |           - |           - |           - |                   - |
|                              Func |     2.780 ns |  0.0060 ns |  0.0053 ns |   0.24 |    0.00 |           - |           - |           - |                   - |
|                          Comparer |     7.881 ns |  0.1915 ns |  0.2205 ns |   0.67 |    0.02 |           - |           - |           - |                   - |
|   EqualByPropertyValuesStructural | 2,875.387 ns | 56.3315 ns | 69.1801 ns | 245.44 |    6.96 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|   EqualByPropertyValuesReferences |   208.874 ns |  0.2324 ns |  0.1940 ns |  17.70 |    0.14 |      0.0367 |           - |           - |               232 B |
| EqualByPropertyValuesWithComparer |   941.902 ns | 18.6590 ns | 35.9495 ns |  79.19 |    3.25 |      0.1202 |      0.0191 |      0.0019 |               765 B |
|      EqualByFieldValuesStructural | 2,783.631 ns |  8.2731 ns |  6.9084 ns | 235.91 |    1.94 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|      EqualByFieldValuesReferences |   220.536 ns |  4.4344 ns | 10.2774 ns |  18.75 |    0.81 |      0.0367 |           - |           - |               232 B |
|    EqualByFieldValuesWithComparer |   895.791 ns |  4.9106 ns |  3.8339 ns |  75.94 |    0.74 |      0.1211 |      0.0248 |      0.0010 |               765 B |
