``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17763.404 (1809/October2018Update/Redstone5)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3324.0


```
|                            Method |       Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|---------------------------------- |-----------:|----------:|----------:|------:|--------:|-------:|-------:|-------:|----------:|
|              this_x_Equals_this_y |  11.066 ns | 0.0840 ns | 0.0786 ns |  1.00 |    0.00 |      - |      - |      - |         - |
|                      ObjectEquals |  11.614 ns | 0.0320 ns | 0.0268 ns |  1.05 |    0.01 |      - |      - |      - |         - |
|                              Func |   2.852 ns | 0.0043 ns | 0.0038 ns |  0.26 |    0.00 |      - |      - |      - |         - |
|                          Comparer |   7.508 ns | 0.0179 ns | 0.0149 ns |  0.68 |    0.00 |      - |      - |      - |         - |
|   EqualByPropertyValuesStructural | 641.658 ns | 5.4413 ns | 4.8235 ns | 57.95 |    0.62 | 0.0725 | 0.0191 | 0.0010 |     461 B |
|   EqualByPropertyValuesReferences | 212.170 ns | 1.2013 ns | 1.1237 ns | 19.17 |    0.18 | 0.0291 |      - |      - |     184 B |
| EqualByPropertyValuesWithComparer | 372.067 ns | 2.3629 ns | 1.9731 ns | 33.58 |    0.22 | 0.0587 | 0.0172 | 0.0005 |     373 B |
|      EqualByFieldValuesStructural | 589.112 ns | 6.9942 ns | 5.8405 ns | 53.18 |    0.57 | 0.0725 | 0.0191 | 0.0010 |     461 B |
|      EqualByFieldValuesReferences | 174.969 ns | 1.5008 ns | 1.3305 ns | 15.80 |    0.17 | 0.0291 |      - |      - |     184 B |
|    EqualByFieldValuesWithComparer | 374.018 ns | 5.0987 ns | 4.7693 ns | 33.80 |    0.33 | 0.0587 | 0.0172 | 0.0005 |     373 B |
