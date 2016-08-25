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
            ForLoop |  3.7193 us | 0.2381 us |   1.00 |      0.00 |      - |     - |     - |               0,25 |
 CopyPropertyValues | 36.8143 us | 2.0822 us |   9.96 |      0.77 | 106.00 |     - |     - |             662,93 |
    CopyFieldValues | 36.4151 us | 1.2592 us |   9.76 |      0.62 | 115.47 |     - |     - |             714,35 |
