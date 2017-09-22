``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410087 Hz, Resolution=293.2477 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0


```
 |                            Method |         Mean |       Error |      StdDev | Scaled | ScaledSD |  Gen 0 |  Gen 1 | Allocated |
 |---------------------------------- |-------------:|------------:|------------:|-------:|---------:|-------:|-------:|----------:|
 |              this_x_Equals_this_y |    13.784 ns |   0.4357 ns |   1.2710 ns |   1.00 |     0.00 |      - |      - |       0 B |
 |                      ObjectEquals |    14.146 ns |   0.4332 ns |   1.2705 ns |   1.03 |     0.13 |      - |      - |       0 B |
 |                              Func |     3.303 ns |   0.1280 ns |   0.3754 ns |   0.24 |     0.04 |      - |      - |       0 B |
 |                          Comparer |     9.167 ns |   0.2635 ns |   0.7768 ns |   0.67 |     0.08 |      - |      - |       0 B |
 |             EqualByPropertyValues | 5,712.234 ns | 128.9200 ns | 376.0656 ns | 417.91 |    47.07 | 0.3281 | 0.0153 |    2079 B |
 | EqualByPropertyValuesWithComparer |    62.376 ns |   1.5309 ns |   4.4657 ns |   4.56 |     0.53 |      - |      - |       0 B |
 |                EqualByFieldValues | 5,752.254 ns | 133.8304 ns | 394.6019 ns | 420.84 |    48.09 | 0.3052 | 0.0153 |    1958 B |
 |    EqualByFieldValuesWIthComparer |    60.518 ns |   1.4175 ns |   4.1574 ns |   4.43 |     0.51 |      - |      - |       0 B |
