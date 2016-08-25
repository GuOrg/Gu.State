```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 ticks, Resolution=410.5309 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=EqualBy  Mode=Throughput  

```
                Method |        Median |      StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------- |-------------- |------------ |------- |---------- |------- |------ |------ |------------------- |
  this_x_Equals_this_y |    14.5017 ns |   0.4015 ns |   1.00 |      0.00 |      - |     - |     - |               0,00 |
          ObjectEquals |    15.9537 ns |   0.3902 ns |   1.10 |      0.04 |      - |     - |     - |               0,00 |
                  Func |     4.1040 ns |   0.2850 ns |   0.29 |      0.02 |      - |     - |     - |               0,00 |
 EqualByPropertyValues | 5,064.5534 ns | 109.7236 ns | 349.12 |     11.73 | 591.02 | 41.71 |     - |             461,15 |
    EqualByFieldValues | 5,192.3416 ns | 129.1687 ns | 357.31 |     12.74 | 528.00 | 19.00 |     - |             389,65 |
