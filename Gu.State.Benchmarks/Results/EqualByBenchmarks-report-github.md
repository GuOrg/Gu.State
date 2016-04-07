```ini

BenchmarkDotNet=v0.9.4.0
OS=Microsoft Windows NT 6.1.7601 Service Pack 1
Processor=Intel(R) Xeon(R) CPU           X5687  @ 3.60GHz, ProcessorCount=8
Frequency=3515781 ticks, Resolution=284.4318 ns, Timer=TSC
HostCLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
JitModules=clrjit-v4.6.1055.0

Type=EqualByBenchmarks  Mode=Throughput  

```
                Method |        Median |      StdDev | Scaled |
---------------------- |-------------- |------------ |------- |
    EqualByFieldValues | 1,232.4269 ns | 110.6756 ns |  68.70 |
 EqualByPropertyValues | 1,386.5390 ns |  30.8171 ns |  77.29 |
          ObjectEquals |    17.9398 ns |   4.8423 ns |   1.00 |
