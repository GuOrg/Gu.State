```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 ticks, Resolution=410.5309 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=CopyArray3D  Mode=Throughput  

```
             Method |     Median |    StdDev | Scaled | Scaled-SD | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |----------- |---------- |------- |---------- |------ |------ |------ |------------------- |
           Forloops | 18.7158 us | 0.2919 us |   1.00 |      0.00 |     - |     - |     - |               1,04 |
 CopyPropertyValues | 41.5591 us | 1.9952 us |   2.25 |      0.11 | 71.00 |     - |     - |             464,13 |
    CopyFieldValues | 40.5945 us | 0.4682 us |   2.18 |      0.04 | 73.37 |     - |     - |             453,09 |
