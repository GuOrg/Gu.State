``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0


```
 |             Method |     Mean |    Error |   StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
 |------------------- |---------:|---------:|---------:|-------:|---------:|-------:|----------:|
 |           ForLoops | 167.2 us | 4.227 us | 12.13 us |   1.00 |     0.00 |      - |       0 B |
 | CopyPropertyValues | 182.1 us | 4.109 us | 11.99 us |   1.09 |     0.11 | 0.2441 |    2364 B |
 |    CopyFieldValues | 194.9 us | 5.634 us | 16.52 us |   1.17 |     0.13 | 0.2441 |    2250 B |
