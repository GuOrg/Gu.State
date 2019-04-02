``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.648 (1803/April2018Update/Redstone4)
Intel Xeon CPU E5-2637 v4 3.50GHz, 2 CPU, 16 logical and 8 physical cores
Frequency=3410070 Hz, Resolution=293.2491 ns, Timer=TSC
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.3362.0


```
|                            Method |         Mean |      Error |     StdDev |  Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|---------------------------------- |-------------:|-----------:|-----------:|-------:|--------:|------------:|------------:|------------:|--------------------:|
|              this_x_Equals_this_y |    11.599 ns |  0.1465 ns |  0.1299 ns |   1.00 |    0.00 |           - |           - |           - |                   - |
|                      ObjectEquals |    12.373 ns |  0.2431 ns |  0.2274 ns |   1.07 |    0.02 |           - |           - |           - |                   - |
|                              Func |     2.776 ns |  0.0960 ns |  0.1180 ns |   0.24 |    0.01 |           - |           - |           - |                   - |
|                          Comparer |     7.637 ns |  0.1890 ns |  0.2829 ns |   0.66 |    0.02 |           - |           - |           - |                   - |
|             EqualByPropertyValues | 3,171.021 ns | 62.5449 ns | 87.6792 ns | 272.81 |    9.11 |      0.2251 |      0.0267 |      0.0076 |              1458 B |
| EqualByPropertyValuesWithComparer | 1,025.328 ns | 16.4699 ns | 14.6001 ns |  88.40 |    1.43 |      0.1373 |      0.0191 |      0.0019 |               869 B |
|                EqualByFieldValues | 2,995.436 ns | 24.8790 ns | 20.7751 ns | 257.88 |    3.24 |      0.2251 |      0.0267 |      0.0076 |              1458 B |
|    EqualByFieldValuesWithComparer | 1,094.956 ns | 20.9878 ns | 25.7749 ns |  94.21 |    1.81 |      0.1373 |      0.0191 |      0.0019 |               869 B |
