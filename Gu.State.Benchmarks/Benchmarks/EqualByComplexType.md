``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410076 Hz, Resolution=293.2486 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |     StdDev |       Median |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.779 ns |  0.1265 ns |  0.1122 ns |    11.782 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    12.688 ns |  0.2536 ns |  0.2118 ns |    12.692 ns |   1.08 |    0.02 |           - |           - |           - |                   - |
|                              Func |     2.794 ns |  0.0071 ns |  0.0063 ns |     2.793 ns |   0.24 |    0.00 |           - |           - |           - |                   - |
|                          Comparer |     7.968 ns |  0.1897 ns |  0.2184 ns |     8.014 ns |   0.68 |    0.02 |           - |           - |           - |                   - |
|   EqualByPropertyValuesStructural | 2,871.609 ns | 55.9404 ns | 76.5719 ns | 2,840.899 ns | 246.78 |    6.36 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|   EqualByPropertyValuesReferences |   212.318 ns |  0.2978 ns |  0.2325 ns |   212.263 ns |  18.04 |    0.16 |      0.0367 |           - |           - |               232 B |
| EqualByPropertyValuesWithComparer |   985.118 ns | 19.1327 ns | 21.2660 ns |   986.793 ns |  83.59 |    1.94 |      0.1202 |      0.0191 |      0.0019 |               765 B |
|      EqualByFieldValuesStructural | 2,884.560 ns |  4.9112 ns |  4.1010 ns | 2,885.096 ns | 244.77 |    2.40 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|      EqualByFieldValuesReferences |   212.876 ns |  4.2368 ns |  4.3508 ns |   212.740 ns |  18.01 |    0.41 |      0.0367 |           - |           - |               232 B |
|    EqualByFieldValuesWithComparer |   929.725 ns | 18.5845 ns | 32.5493 ns |   907.736 ns |  81.02 |    2.67 |      0.1202 |      0.0191 |      0.0019 |               765 B |
