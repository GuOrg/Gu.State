```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 ticks, Resolution=410.5309 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=CopyList  Mode=Throughput  

```
             Method |     Median |    StdDev | Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |----------- |---------- |------- |---------- |------- |------ |------ |------------------- |
            ForLoop |  3.8179 us | 0.1557 us |   1.00 |      0.00 |      - |     - |     - |               0,21 |
 CopyPropertyValues | 37.3312 us | 1.0582 us |   9.73 |      0.45 | 125.65 |     - |     - |             730,51 |
    CopyFieldValues | 39.2657 us | 1.8825 us |  10.30 |      0.62 | 130.00 |     - |     - |             754,42 |
