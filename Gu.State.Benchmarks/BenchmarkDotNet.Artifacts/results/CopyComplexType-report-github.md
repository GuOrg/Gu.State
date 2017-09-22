``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Xeon CPU E5-2637 v4 3.50GHzIntel Xeon CPU E5-2637 v4 3.50GHz, ProcessorCount=16
Frequency=3410107 Hz, Resolution=293.2459 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2114.0


```
 |               Method |         Mean |       Error |      StdDev |   Scaled | ScaledSD |  Gen 0 |  Gen 1 |  Gen 2 | Allocated |
 |--------------------- |-------------:|------------:|------------:|---------:|---------:|-------:|-------:|-------:|----------:|
 | ManualImplementation |     5.382 ns |   0.2440 ns |   0.7157 ns |     1.00 |     0.00 |      - |      - |      - |       0 B |
 |   CopyPropertyValues | 8,314.441 ns |  31.6289 ns |  28.0382 ns | 1,571.26 |   202.85 | 0.3204 | 0.0153 |      - |    2035 B |
 |      CopyFieldValues | 7,066.030 ns | 142.3303 ns | 389.6270 ns | 1,335.33 |   187.48 | 0.2975 | 0.0305 | 0.0076 |    1914 B |
