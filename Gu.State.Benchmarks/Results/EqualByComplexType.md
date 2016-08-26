```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 ticks, Resolution=410.5309 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=EqualByComplexType  Mode=Throughput  

```
                Method |        Median |      StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
---------------------- |-------------- |------------ |------- |---------- |------- |------ |------ |------------------- |
  this_x_Equals_this_y |    14.9189 ns |   1.0045 ns |   1.00 |      0.00 |      - |     - |     - |               0,00 |
          ObjectEquals |    16.6291 ns |   0.5895 ns |   1.10 |      0.07 |      - |     - |     - |               0,00 |
                  Func |     4.2441 ns |   0.2471 ns |   0.28 |      0.02 |      - |     - |     - |               0,00 |
              Comparer |    12.5282 ns |   0.4018 ns |   0.83 |      0.05 |      - |     - |     - |               0,00 |
 EqualByPropertyValues | 5,370.1510 ns | 243.8740 ns | 352.61 |     25.76 | 287.02 | 20.08 |     - |             439,24 |
    EqualByFieldValues | 5,529.2634 ns | 262.3301 ns | 369.65 |     27.31 | 232.00 | 17.00 |     - |             361,82 |
