``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0


```
 |                            Method |         Mean |       Error |      StdDev | Scaled | ScaledSD |  Gen 0 |  Gen 1 | Allocated |
 |---------------------------------- |-------------:|------------:|------------:|-------:|---------:|-------:|-------:|----------:|
 |              this_x_Equals_this_y |    12.895 ns |   0.2949 ns |   0.7870 ns |   1.00 |     0.00 |      - |      - |       0 B |
 |                      ObjectEquals |    13.756 ns |   0.3842 ns |   1.1268 ns |   1.07 |     0.11 |      - |      - |       0 B |
 |                              Func |     3.243 ns |   0.1095 ns |   0.3195 ns |   0.25 |     0.03 |      - |      - |       0 B |
 |                          Comparer |    10.005 ns |   0.4070 ns |   1.1677 ns |   0.78 |     0.10 |      - |      - |       0 B |
 |             EqualByPropertyValues | 7,046.775 ns | 296.7671 ns | 865.6835 ns | 548.44 |    74.47 | 0.3281 | 0.0153 |    2079 B |
 | EqualByPropertyValuesWithComparer |    61.290 ns |   1.2633 ns |   3.4154 ns |   4.77 |     0.39 |      - |      - |       0 B |
 |                EqualByFieldValues | 7,536.336 ns |  11.4995 ns |   8.3149 ns | 586.54 |    34.44 | 0.3052 | 0.0153 |    1958 B |
 |    EqualByFieldValuesWIthComparer |    60.269 ns |   1.2436 ns |   3.1878 ns |   4.69 |     0.37 |      - |      - |       0 B |
