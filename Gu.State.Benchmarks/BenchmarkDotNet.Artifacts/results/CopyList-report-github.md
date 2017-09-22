``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0


```
 |             Method |      Mean |     Error |    StdDev |    Median | Scaled | ScaledSD |  Gen 0 |  Gen 1 | Allocated |
 |------------------- |----------:|----------:|----------:|----------:|-------:|---------:|-------:|-------:|----------:|
 |            ForLoop |  2.434 us | 0.0532 us | 0.1570 us |  2.454 us |   1.00 |     0.00 |      - |      - |       0 B |
 | CopyPropertyValues | 30.932 us | 0.9580 us | 2.7177 us | 30.142 us |  12.76 |     1.40 | 0.4883 | 0.0610 |    3291 B |
 |    CopyFieldValues | 31.864 us | 0.8034 us | 2.3689 us | 31.944 us |  13.15 |     1.30 | 0.4883 | 0.0610 |    3275 B |
