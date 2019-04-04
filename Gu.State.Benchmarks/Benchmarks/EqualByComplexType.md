``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410073 Hz, Resolution=293.2489 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |      StdDev |       Median |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|------------:|-------------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    10.889 ns |  0.0819 ns |   0.0726 ns |    10.866 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    12.102 ns |  0.2780 ns |   0.2464 ns |    12.181 ns |   1.11 |    0.02 |           - |           - |           - |                   - |
|                              Func |     2.751 ns |  0.0046 ns |   0.0041 ns |     2.751 ns |   0.25 |    0.00 |           - |           - |           - |                   - |
|                          Comparer |     7.561 ns |  0.1832 ns |   0.2181 ns |     7.503 ns |   0.69 |    0.02 |           - |           - |           - |                   - |
|             EqualByPropertyValues | 2,765.808 ns | 54.8479 ns |  75.0764 ns | 2,772.512 ns | 255.67 |    6.48 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
| EqualByPropertyValuesWithComparer |   924.264 ns |  2.3861 ns |   2.1152 ns |   924.966 ns |  84.88 |    0.62 |      0.1211 |      0.0248 |      0.0010 |               765 B |
|                EqualByFieldValues | 2,943.338 ns | 58.6004 ns | 124.8822 ns | 2,946.429 ns | 262.91 |   10.77 |      0.2060 |      0.0153 |      0.0076 |              1351 B |
|    EqualByFieldValuesWithComparer |   988.969 ns | 19.6761 ns |  37.4358 ns |   972.992 ns |  90.66 |    3.08 |      0.1202 |      0.0191 |      0.0019 |               765 B |
