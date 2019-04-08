``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.404 (1809/October2018Update/Redstone5)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0


```
|                            Method |       Mean |      Error |     StdDev | Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-----------:|-----------:|-----------:|------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |  11.154 ns |  0.1099 ns |  0.0974 ns |  1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |  12.131 ns |  0.2095 ns |  0.2151 ns |  1.09 |    0.02 |           - |           - |           - |                   - |
|                              Func |   2.609 ns |  0.0940 ns |  0.1255 ns |  0.23 |    0.01 |           - |           - |           - |                   - |
|                          Comparer |   8.468 ns |  0.2066 ns |  0.2896 ns |  0.75 |    0.03 |           - |           - |           - |                   - |
|   EqualByPropertyValuesStructural | 630.253 ns | 12.3611 ns | 21.3222 ns | 56.66 |    2.39 |      0.0801 |      0.0210 |      0.0010 |               509 B |
|   EqualByPropertyValuesReferences | 235.635 ns |  4.6621 ns |  8.8700 ns | 21.49 |    0.87 |      0.0367 |           - |           - |               232 B |
| EqualByPropertyValuesWithComparer | 362.991 ns |  6.6945 ns |  5.9345 ns | 32.54 |    0.44 |      0.0587 |      0.0172 |      0.0005 |               373 B |
|      EqualByFieldValuesStructural | 644.606 ns | 15.7371 ns | 16.8385 ns | 57.98 |    1.89 |      0.0801 |      0.0210 |      0.0010 |               509 B |
|      EqualByFieldValuesReferences | 215.783 ns |  4.0967 ns |  3.6316 ns | 19.35 |    0.39 |      0.0367 |           - |           - |               232 B |
|    EqualByFieldValuesWithComparer | 361.385 ns |  7.8213 ns | 12.4054 ns | 31.93 |    0.89 |      0.0587 |      0.0172 |      0.0005 |               373 B |
