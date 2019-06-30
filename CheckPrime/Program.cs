using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrime
{
    class Program
    {
        // cf https://en.wikipedia.org/wiki/Prime-counting_function

        private static readonly List<long> BenchMark = new List<long>()
        { 4, 25, 168, 1229, 9592, 78498, 664579, 5761455, 50847534,
            455052511, 4118054813, 37607912018};

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            var primes = MathPrime.GeneratePrimes(31);
            int pos = 0;
            for (ulong N = 10; N <= 10_000_000_000; N *= 10)
            {
                sw.Restart();
                var count = MathPrime.Count(N);
                sw.Stop();
                Console.WriteLine($"{N} => {count} {sw.ElapsedMilliseconds} ms ok?={count==BenchMark[pos++]}");                
            }
        }
    }
}
