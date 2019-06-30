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
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            var primes = MathPrime.GeneratePrimes(31);
            for (ulong N = 10; N <= 10_000_000_000; N *= 10)
            {
                sw.Restart();
                var count = MathPrime.Count(N);
                sw.Stop();
                Console.WriteLine($"{N} => {count} {sw.ElapsedMilliseconds} ms");                
            }
        }
    }
}
