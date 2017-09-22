``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0


```
 |             Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |  Gen 0 |  Gen 1 | Allocated |
 |------------------- |---------:|----------:|----------:|-------:|---------:|-------:|-------:|----------:|
 |           Forloops | 15.64 us | 0.3096 us | 0.7711 us |   1.00 |     0.00 |      - |      - |       0 B |
 | CopyPropertyValues | 35.37 us | 1.0945 us | 3.1404 us |   2.27 |     0.23 | 0.3052 | 0.0610 |    2353 B |
 |    CopyFieldValues | 34.98 us | 0.8380 us | 2.4311 us |   2.24 |     0.19 | 0.3052 | 0.0610 |    2239 B |
