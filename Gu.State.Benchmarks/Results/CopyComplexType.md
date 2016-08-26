```ini

Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-3667U CPU 2.00GHz, ProcessorCount=4
Frequency=2435870 ticks, Resolution=410.5309 ns, Timer=TSC
CLR=MS.NET 4.0.30319.42000, Arch=32-bit RELEASE
GC=Concurrent Workstation
JitModules=clrjit-v4.6.1586.0

Type=CopyComplexType  Mode=Throughput  

```
               Method |        Median |      StdDev |   Scaled | Scaled-SD |  Gen 0 | Gen 1 | Gen 2 | Bytes Allocated/Op |
--------------------- |-------------- |------------ |--------- |---------- |------- |------ |------ |------------------- |
 ManualImplementation |     5.4568 ns |   0.2594 ns |     1.00 |      0.00 |      - |     - |     - |               0,00 |
   CopyPropertyValues | 6,589.9061 ns | 133.6265 ns | 1,196.62 |     54.01 | 231.00 | 18.00 |     - |             392,70 |
      CopyFieldValues | 6,771.1170 ns | 452.0046 ns | 1,265.85 |     95.87 | 244.00 | 19.00 |     - |             389,02 |
