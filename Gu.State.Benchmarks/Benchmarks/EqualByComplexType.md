``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Core i7-7500U CPU 2.70GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
Frequency=2835936 Hz, Resolution=352.6173 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |      StdDev |       Median |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|------------:|-------------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.968 ns |  0.2866 ns |   0.4377 ns |    11.817 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    11.987 ns |  0.1060 ns |   0.0885 ns |    11.954 ns |   0.98 |    0.04 |           - |           - |           - |                   - |
|                              Func |     3.405 ns |  0.1683 ns |   0.4830 ns |     3.207 ns |   0.27 |    0.04 |           - |           - |           - |                   - |
|                          Comparer |     9.746 ns |  0.2295 ns |   0.4583 ns |     9.516 ns |   0.83 |    0.06 |           - |           - |           - |                   - |
|   EqualByPropertyValuesStructural | 3,284.644 ns | 63.1378 ns |  72.7096 ns | 3,280.671 ns | 271.81 |    8.78 |      0.8583 |      0.0076 |           - |              1822 B |
|   EqualByPropertyValuesReferences |   774.830 ns | 15.4156 ns |  14.4198 ns |   773.480 ns |  63.51 |    3.10 |      0.3586 |           - |           - |               752 B |
| EqualByPropertyValuesWithComparer |    64.044 ns |  0.4632 ns |   0.4106 ns |    63.977 ns |   5.24 |    0.22 |           - |           - |           - |                   - |
|      EqualByFieldValuesStructural | 3,407.489 ns | 36.3296 ns |  33.9827 ns | 3,414.012 ns | 279.26 |   12.32 |      0.8087 |      0.0076 |           - |              1740 B |
|      EqualByFieldValuesReferences | 1,170.540 ns | 59.1990 ns | 171.7471 ns | 1,145.042 ns |  92.42 |   10.26 |      0.3204 |           - |           - |               672 B |
|    EqualByFieldValuesWithComparer |    69.749 ns |  1.7094 ns |   1.5153 ns |    69.596 ns |   5.70 |    0.28 |           - |           - |           - |                   - |
