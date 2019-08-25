using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Test

namespace CheckPrime
{
    class TestCase
    {
        public TestCase(ulong n, ulong bench) { N = n; Bench = bench; }
        public ulong N { get; set; }
        public ulong Bench { get; set; }
    }

    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        // cf https://en.wikipedia.org/wiki/Prime-counting_function

        private static readonly List<TestCase> BenchMark = new List<TestCase>()
        {
            new TestCase(                 4,            2), // 0
            new TestCase(                10,            4), // 1 
            new TestCase(               100,           25), // 2
            new TestCase(             1_000,          168), // 3
            new TestCase(            10_000,         1229), // 4
            new TestCase(           100_000,         9592), // 5
            new TestCase(         1_000_000,        78498), // 6
            new TestCase(        10_000_000,       664579), // 7
            new TestCase(       100_000_000,      5761455), // 8
            new TestCase(     1_000_000_000,     50847534), // 9
            new TestCase(    10_000_000_000,    455052511), // 10
            new TestCase(   100_000_000_000,   4118054813), // 11
            new TestCase( 1_000_000_000_000,  37607912018), // 12
            new TestCase(10_000_000_000_000, 346065536839)  // 13 pb?
        };

        static void Main(string[] args)
        {
            try
            {
                Logger.Info("==================================================");
                var sw = new Stopwatch();
                long totalTime = 0;
                int testCase = 11;
                for (int i = 0; i <= Math.Min(testCase, BenchMark.Count); i++)
                {
                    Logger.Info($"====== {BenchMark[i].N} (10^{Math.Log10(BenchMark[i].N)}) => ");
                    sw.Restart();
                    var count = MathPrime.Count(BenchMark[i].N);
                    sw.Stop();
                    totalTime += sw.ElapsedMilliseconds;
                    bool isOk = (count == (long)BenchMark[i].Bench);
                    Logger.Info($"Count={count} ok?={isOk}  ({sw.ElapsedMilliseconds} ms)");
                    if (!isOk)
                        throw new Exception("Check is incorrect!!!");
                }
                Logger.Info($"Total time: {totalTime / 1000.0:0.00} s");
                var myProcess = Process.GetCurrentProcess();
                Logger.Info($"Max working set: {myProcess.PeakWorkingSet64 / (1024.0 * 1024.0):0.0} Mo");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}