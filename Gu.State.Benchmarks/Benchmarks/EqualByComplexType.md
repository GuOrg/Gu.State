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
---------------------------------- |-------------- |------------ |------- |---------- |------- |------ |------ |------------------- |
              this_x_Equals_this_y |    14.3928 ns |   0.9434 ns |   1.00 |      0.00 |      - |     - |     - |               0,00 |
                      ObjectEquals |    16.0235 ns |   0.2249 ns |   1.11 |      0.05 |      - |     - |     - |               0,00 |
                              Func |     4.0487 ns |   0.1621 ns |   0.28 |      0.02 |      - |     - |     - |               0,00 |
                          Comparer |    11.7366 ns |   0.6902 ns |   0.82 |      0.06 |      - |     - |     - |               0,00 |
             EqualByPropertyValues | 4,952.9816 ns | 238.5484 ns | 345.23 |     22.38 | 586.00 | 41.00 |     - |             430,46 |
 EqualByPropertyValuesWithComparer |    83.7546 ns |   2.1942 ns |   5.82 |      0.30 |      - |     - |     - |               0,00 |
                EqualByFieldValues | 5,320.1345 ns | 110.3716 ns | 367.54 |     18.14 | 633.33 | 22.73 |     - |             438,04 |
    EqualByFieldValuesWIthComparer |    83.7185 ns |   0.9609 ns |   5.79 |      0.27 |      - |     - |     - |               0,00 |
