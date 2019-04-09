``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.404 (1809/October2018Update/Redstone5)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0


```
|                            Method |       Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------------- |-----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
|              this_x_Equals_this_y |  13.871 ns | 0.1273 ns | 0.1063 ns |  1.00 |    0.00 |      - |     - |     - |         - |
|                      ObjectEquals |  14.322 ns | 0.1295 ns | 0.1211 ns |  1.03 |    0.01 |      - |     - |     - |         - |
|                              Func |   4.984 ns | 0.0508 ns | 0.0475 ns |  0.36 |    0.00 |      - |     - |     - |         - |
|                          Comparer |  10.184 ns | 0.1395 ns | 0.1165 ns |  0.73 |    0.01 |      - |     - |     - |         - |
|   EqualByPropertyValuesStructural | 208.441 ns | 1.3624 ns | 1.0637 ns | 15.04 |    0.15 | 0.0291 |     - |     - |     184 B |
|   EqualByPropertyValuesReferences | 206.520 ns | 0.9757 ns | 0.9127 ns | 14.90 |    0.14 | 0.0291 |     - |     - |     184 B |
| EqualByPropertyValuesWithComparer |  84.044 ns | 0.4908 ns | 0.4098 ns |  6.06 |    0.05 | 0.0151 |     - |     - |      96 B |
|      EqualByFieldValuesStructural | 178.514 ns | 2.5806 ns | 2.2876 ns | 12.88 |    0.16 | 0.0291 |     - |     - |     184 B |
|      EqualByFieldValuesReferences | 178.195 ns | 1.3859 ns | 1.2285 ns | 12.86 |    0.10 | 0.0291 |     - |     - |     184 B |
|    EqualByFieldValuesWithComparer |  83.627 ns | 0.8751 ns | 0.7758 ns |  6.03 |    0.06 | 0.0151 |     - |     - |      96 B |
