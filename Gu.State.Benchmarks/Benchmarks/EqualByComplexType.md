``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410076 Hz, Resolution=293.2486 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |     StdDev |       Median |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|-----------:|-------------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    12.709 ns |  0.1923 ns |  0.1705 ns |    12.654 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    12.053 ns |  0.2750 ns |  0.3477 ns |    11.835 ns |   0.96 |    0.03 |           - |           - |           - |                   - |
|                              Func |     2.773 ns |  0.0090 ns |  0.0080 ns |     2.771 ns |   0.22 |    0.00 |           - |           - |           - |                   - |
|                          Comparer |     7.740 ns |  0.1888 ns |  0.2318 ns |     7.745 ns |   0.61 |    0.02 |           - |           - |           - |                   - |
|   EqualByPropertyValuesStructural | 2,851.728 ns | 29.6178 ns | 26.2554 ns | 2,837.853 ns | 224.41 |    3.06 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|   EqualByPropertyValuesReferences |   215.176 ns |  4.1996 ns |  4.4935 ns |   215.095 ns |  16.96 |    0.48 |      0.0367 |           - |           - |               232 B |
| EqualByPropertyValuesWithComparer |   948.558 ns | 18.7358 ns | 31.8148 ns |   932.742 ns |  75.60 |    2.61 |      0.1202 |      0.0191 |      0.0019 |               765 B |
|      EqualByFieldValuesStructural | 2,935.903 ns | 55.2519 ns | 61.4124 ns | 2,921.848 ns | 230.54 |    6.92 |      0.2060 |      0.0229 |      0.0076 |              1352 B |
|      EqualByFieldValuesReferences |   214.720 ns |  4.2484 ns |  5.3729 ns |   214.403 ns |  16.82 |    0.33 |      0.0367 |           - |           - |               232 B |
|    EqualByFieldValuesWithComparer |   924.714 ns | 13.8758 ns | 11.5869 ns |   917.808 ns |  72.70 |    1.40 |      0.1202 |      0.0191 |      0.0019 |               765 B |
