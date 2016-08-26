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
           Forloops | 20.1044 us | 1.2283 us |   1.00 |      0.00 |     - |     - |     - |               0,75 |
 CopyPropertyValues | 42.1029 us | 1.5257 us |   2.10 |      0.14 | 71.00 |     - |     - |             435,39 |
    CopyFieldValues | 41.1301 us | 0.3105 us |   2.03 |      0.12 | 81.20 |     - |     - |             465,32 |
