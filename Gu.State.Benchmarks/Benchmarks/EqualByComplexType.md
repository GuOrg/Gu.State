``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.404 (1809/October2018Update/Redstone5)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0


```
|                            Method |       Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------------- |-----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
|              this_x_Equals_this_y |  11.708 ns | 0.2262 ns | 0.2005 ns |  1.00 |    0.00 |      - |     - |     - |         - |
|                      ObjectEquals |  12.402 ns | 0.2827 ns | 0.2777 ns |  1.06 |    0.02 |      - |     - |     - |         - |
|                              Func |   2.810 ns | 0.0504 ns | 0.0447 ns |  0.24 |    0.00 |      - |     - |     - |         - |
|                          Comparer |   7.287 ns | 0.0438 ns | 0.0388 ns |  0.62 |    0.01 |      - |     - |     - |         - |
|   EqualByPropertyValuesStructural | 208.011 ns | 1.2129 ns | 1.0752 ns | 17.77 |    0.27 | 0.0291 |     - |     - |     184 B |
|   EqualByPropertyValuesReferences | 208.218 ns | 1.3508 ns | 1.1280 ns | 17.76 |    0.33 | 0.0291 |     - |     - |     184 B |
| EqualByPropertyValuesWithComparer |  83.147 ns | 0.8150 ns | 0.6806 ns |  7.09 |    0.13 | 0.0151 |     - |     - |      96 B |
|      EqualByFieldValuesStructural | 178.866 ns | 0.7655 ns | 0.7160 ns | 15.29 |    0.26 | 0.0291 |     - |     - |     184 B |
|      EqualByFieldValuesReferences | 176.407 ns | 1.2488 ns | 1.0428 ns | 15.05 |    0.30 | 0.0291 |     - |     - |     184 B |
|    EqualByFieldValuesWithComparer |  81.722 ns | 0.5377 ns | 0.4490 ns |  6.97 |    0.12 | 0.0151 |     - |     - |      96 B |
