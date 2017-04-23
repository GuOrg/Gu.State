``` ini

BenchmarkDotNet=v0.10.4, OS=Windows 10.0.14393
Processor=Intel Core i7-3667U CPU 2.00GHz (Ivy Bridge), ProcessorCount=4
Frequency=2435873 Hz, Resolution=410.5304 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1637.0
  DefaultJob : Clr 4.0.30319.42000, 32bit LegacyJIT-v4.6.1637.0


```
 |                            Method |          Mean |      Error |     StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
 |---------------------------------- |--------------:|-----------:|-----------:|-------:|---------:|-------:|----------:|
 |              this_x_Equals_this_y |    14.9570 ns |  0.3473 ns |  0.8453 ns |   1.00 |     0.00 |      - |      0 kB |
 |                      ObjectEquals |    17.8380 ns |  0.4971 ns |  1.4501 ns |   1.20 |     0.12 |      - |      0 kB |
 |                              Func |     4.6834 ns |  0.1469 ns |  0.3947 ns |   0.31 |     0.03 |      - |      0 kB |
 |                          Comparer |    12.0413 ns |  0.0587 ns |  0.0549 ns |   0.81 |     0.04 |      - |      0 kB |
 |             EqualByPropertyValues | 4,771.8490 ns | 55.2336 ns | 51.6656 ns | 320.00 |    17.49 | 0.3596 |   1.02 kB |
 | EqualByPropertyValuesWithComparer |    81.6122 ns |  0.8697 ns |  0.8135 ns |   5.47 |     0.30 |      - |      0 kB |
 |                EqualByFieldValues | 4,849.9549 ns | 74.6506 ns | 69.8282 ns | 325.24 |    18.03 | 0.3489 |   0.96 kB |
 |    EqualByFieldValuesWIthComparer |    81.3233 ns |  0.5104 ns |  0.4774 ns |   5.45 |     0.29 |      - |      0 kB |
