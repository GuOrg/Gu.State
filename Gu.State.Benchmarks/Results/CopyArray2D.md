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
           ForLoops | 186.1020 us |  3.1020 us |   1.00 |      0.00 |     - |     - |     - |               8,11 |
 CopyPropertyValues | 235.7105 us | 30.6174 us |   1.27 |      0.16 |     - |     - |     - |             440,37 |
    CopyFieldValues | 208.6347 us |  4.3792 us |   1.12 |      0.03 |     - |     - |     - |             319,25 |
