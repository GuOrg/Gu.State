// ReSharper disable UnusedMember.Local
namespace Gu.State.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main()
        {
            foreach (var summary in RunSingle<EqualByComplexType>())
            {
                CopyResult(summary);
            }
        }

        private static IEnumerable<Summary> RunAll() => new BenchmarkSwitcher(typeof(Program).Assembly).RunAll();

        private static IEnumerable<Summary> RunSingle<T>() => new[] { BenchmarkRunner.Run<T>() };

        private static void CopyResult(Summary summary)
        {
            var trimmedTitle = summary.Title.Split('.').Last().Split('-').First();
            Console.WriteLine(trimmedTitle);
            var sourceFileName = FindMdFile();
            var destinationFileName = Path.ChangeExtension(FindCsFile(), ".md");
            Console.WriteLine($"Copy: {sourceFileName} -> {destinationFileName}");
            File.Copy(sourceFileName, destinationFileName, overwrite: true);

            string FindMdFile()
            {
                return Directory.EnumerateFiles(summary.ResultsDirectoryPath, $"*{trimmedTitle}-report-github.md")
                                .Single();
            }

            string FindCsFile()
            {
                return Directory.EnumerateFiles(
                                    AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.RemoveEmptyEntries).First(),
                                    $"{trimmedTitle}.cs",
                                    SearchOption.AllDirectories)
                                .Single();
            }
        }
    }
}
