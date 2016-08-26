```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 ticks, Resolution=410.5309 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=CopyArray2D  Mode=Throughput  

```
             Method |      Median |     StdDev | Scaled | Scaled-SD | Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
------------------- |------------ |----------- |------- |---------- |------ |------ |------ |------------------- |
           ForLoops | 188.7115 us |  9.9139 us |   1.00 |      0.00 |     - |     - |     - |               8,51 |
 CopyPropertyValues | 215.8799 us |  8.4844 us |   1.14 |      0.07 |     - |     - |     - |             436,59 |
    CopyFieldValues | 217.1620 us | 11.5810 us |   1.15 |      0.08 |     - |     - |     - |             485,65 |
