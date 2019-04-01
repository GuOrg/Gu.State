``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410070 Hz, Resolution=293.2491 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |      StdDev |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|------------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.847 ns |  0.2397 ns |   0.2125 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    11.942 ns |  0.2264 ns |   0.2007 ns |   1.01 |    0.02 |           - |           - |           - |                   - |
|                              Func |     2.855 ns |  0.0929 ns |   0.1208 ns |   0.24 |    0.01 |           - |           - |           - |                   - |
|                          Comparer |     8.771 ns |  0.1568 ns |   0.1310 ns |   0.74 |    0.02 |           - |           - |           - |                   - |
|             EqualByPropertyValues | 3,088.538 ns | 62.4667 ns |  64.1487 ns | 261.67 |    6.49 |      0.2213 |      0.0267 |      0.0076 |              1450 B |
| EqualByPropertyValuesWithComparer |    59.201 ns |  0.1823 ns |   0.1522 ns |   5.00 |    0.10 |           - |           - |           - |                   - |
|                EqualByFieldValues | 3,077.467 ns | 62.2499 ns | 105.7050 ns | 260.54 |    9.62 |      0.2213 |      0.0267 |      0.0076 |              1450 B |
|    EqualByFieldValuesWithComparer |    58.016 ns |  0.1769 ns |   0.1381 ns |   4.91 |    0.09 |           - |           - |           - |                   - |
