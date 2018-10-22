``` ini

BenchmarkDotNet=v0.11.1, OS=Windows 10.0.17134.228 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz (Max: 3.49GHz), 2 CPU, 16 logical and 8 physical cores
Frequency=3410070 Hz, Resolution=293.2491 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3132.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3132.0


```
|                            Method |         Mean |      Error |      StdDev |       Median | Scaled | ScaledSD |  Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|---------------------------------- |-------------:|-----------:|------------:|-------------:|-------:|---------:|-------:|-------:|-------:|----------:|
|              this_x_Equals_this_y |    12.060 ns |  0.2667 ns |   0.2619 ns |    11.928 ns |   1.00 |     0.00 |      - |      - |      - |       0 B |
|                      ObjectEquals |    12.643 ns |  0.0706 ns |   0.0552 ns |    12.616 ns |   1.05 |     0.02 |      - |      - |      - |       0 B |
|                              Func |     2.831 ns |  0.0888 ns |   0.0741 ns |     2.804 ns |   0.23 |     0.01 |      - |      - |      - |       0 B |
|                          Comparer |     8.493 ns |  0.2031 ns |   0.2641 ns |     8.329 ns |   0.70 |     0.03 |      - |      - |      - |       0 B |
|             EqualByPropertyValues | 3,446.281 ns | 50.6514 ns |  47.3794 ns | 3,444.916 ns | 285.89 |     7.03 | 0.2899 | 0.0229 | 0.0038 |    1862 B |
| EqualByPropertyValuesWithComparer |    52.719 ns |  1.0787 ns |   1.1989 ns |    53.187 ns |   4.37 |     0.13 |      - |      - |      - |       0 B |
|                EqualByFieldValues | 3,613.586 ns | 71.6888 ns | 170.3760 ns | 3,484.960 ns | 299.76 |    15.34 | 0.2670 | 0.0076 |      - |    1740 B |
|    EqualByFieldValuesWithComparer |    51.192 ns |  0.3647 ns |   0.3046 ns |    51.302 ns |   4.25 |     0.09 |      - |      - |      - |       0 B |
