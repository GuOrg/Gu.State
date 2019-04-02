``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410074 Hz, Resolution=293.2488 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |     StdDev |       Median |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.327 ns |  0.2603 ns |  0.3098 ns |    11.181 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    11.695 ns |  0.2860 ns |  0.2937 ns |    11.606 ns |   1.03 |    0.04 |           - |           - |           - |                   - |
|                              Func |     2.926 ns |  0.0919 ns |  0.1561 ns |     2.843 ns |   0.26 |    0.01 |           - |           - |           - |                   - |
|                          Comparer |     8.639 ns |  0.0570 ns |  0.0476 ns |     8.625 ns |   0.77 |    0.02 |           - |           - |           - |                   - |
|             EqualByPropertyValues | 2,790.896 ns | 54.8308 ns | 75.0530 ns | 2,775.790 ns | 246.09 |    9.20 |      0.2213 |      0.0267 |      0.0076 |              1450 B |
| EqualByPropertyValuesWithComparer |   988.430 ns | 19.4244 ns | 33.5061 ns |   988.451 ns |  86.94 |    3.96 |      0.1354 |      0.0191 |      0.0019 |               861 B |
|                EqualByFieldValues | 2,772.017 ns | 52.8750 ns | 62.9439 ns | 2,744.655 ns | 244.86 |    7.26 |      0.2213 |      0.0267 |      0.0076 |              1450 B |
|    EqualByFieldValuesWithComparer |   968.651 ns | 19.1132 ns | 22.0108 ns |   958.600 ns |  85.73 |    3.42 |      0.1354 |      0.0191 |      0.0019 |               861 B |
