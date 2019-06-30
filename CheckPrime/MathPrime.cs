using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckPrime
{
    public class MathPrime
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(MathPrime));

        private const int UlongBits = 64;
        private const int UlongShift = 6; // 2^6=64
        private const ulong UlongMask = 63; // 5 bits set to 1

        private static void SetMultiple(ulong k, ulong[] b)
        {
            ulong q = (ulong)(k >> UlongShift);
            int r = (int)(k & UlongMask);
            b[q] |= (1ul << r);
        }

        private static bool IsMultiple(ulong k, ulong[] b)
        {
            ulong q = (ulong)(k >> UlongShift);
            int r = (int)(k & UlongMask);
            return (b[q] & (1ul << r)) != 0;
        }

        // Generate ordered prime list up to N
        public static List<ulong> GeneratePrimes(ulong N)
        {
            var b = new ulong[(N / UlongBits) + 1];

            // Init table
            ulong sqrtN = (ulong)Math.Sqrt((double)N) + 1;
            SetMultiple(0, b);
            SetMultiple(1, b);
            ulong p = 2;
            do
            {
                for (ulong m = p * p; m <= N; m += p)
                {
                    SetMultiple(m, b);
                }
                p++;
                while (p <= sqrtN && IsMultiple(p, b))
                {
                    p++;
                }
            }
            while (p <= sqrtN);
            var primes = new List<ulong>((int)(N / Math.Log(N)));
            for (ulong k = 0; k <= N; k++)
            {
                if (!IsMultiple(k, b))
                {
                    primes.Add(k);
                }
            }
            return primes;
        }

        // Count number of primes <=N
        //  primes = N - Count(composites)
        // number of multiples of pi = [N/pi] with pi*pi <=N
        // But number of multiples of pi.pj counted twice remove [N/pi.pj]
        // Final formulae
        // (N - 1) - [N/pi] + [N/pi.pj] - [N/pi.pj.pk] + count(pi)  (N-1) because range is (2 ... N)

        // Max ulong= 18_446_744_073_709_551_615 ~ 1.8*10^19
        // Max long =  9_223_372_036_854_775_807 ~  9*10^18

        // Cas N=10^13 
        // sqrtN=10^6.5
        // N*sqrt(N) =10^19.5 = 3.16*10^19 depasse ulong!!

        public static long Count(ulong N)
        {
            if (N < 4)
                return 0;
            var sqrtN = (ulong)Math.Sqrt(N);
            if (!(sqrtN * sqrtN <= N && N < (sqrtN + 1) * (sqrtN + 1)))
                throw new Exception("Invalid square root!!"); // Check float approx
            var primes = GeneratePrimes(sqrtN);

            Logger.Info($"sqrtN = {sqrtN}");
            Logger.Info($"primes.Count: {primes.Count}");
            Logger.Info($"Last prime: {primes[primes.Count-1]}");

            var Ndiv2 = N / 2 + 1;

            var productPrimes = new ConcurrentBag<ProductPrime>();
            productPrimes.Add(new ProductPrime() { InvProduct = N, IndexLastPrime = -1 });
                        
            long totalSum = 0;
            long sign = +1;
            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            do
            {
                var newProductPrimes = new ConcurrentBag<ProductPrime>();
                ulong sum = 0;
                object sumMutex = new object();
                Parallel.ForEach(productPrimes, parallelOptions, (productPrime) =>
                {
                    int primeIndex = productPrime.IndexLastPrime + 1;
                    ulong localSum = 0;
                    while (primeIndex < primes.Count)
                    {
                        // Calculer les bornes tq pk~invProduct, 2*pj ~ InvProduct, 3*pi ~ InvProduct
                        // x > pk => 0
                        // x ds [pj,pk] => 1
                        // x ds [pi,pj] => 2

                        if (productPrime.InvProduct >= 2 * primes[primeIndex])
                        {
                            ulong newInvProduct = productPrime.InvProduct / primes[primeIndex];
                            localSum += newInvProduct;
                            if (primeIndex + 1 < primes.Count && newInvProduct >= primes[primeIndex + 1]) 
                                newProductPrimes.Add(new ProductPrime() { InvProduct = newInvProduct, IndexLastPrime = primeIndex });
                            primeIndex++;
                        }
                        else if (productPrime.InvProduct >= primes[primeIndex])
                        {
                            localSum += 1;
                            primeIndex++;
                        }
                        else
                            primeIndex = primes.Count;
                    }
                    lock(sumMutex)
                    {
                        sum += localSum;                        
                    } 
                });
                
                totalSum += sign * (long)sum;
                sign = -sign;
                Logger.Info($"totalSum= {totalSum} sum={sum}");

                // point to new list
                productPrimes = newProductPrimes;
                Logger.Info($"New count={newProductPrimes.Count}");
                Logger.Info("");                
            }
            while (productPrimes.Count > 0);
            return (long)(N - 1) - totalSum + primes.Count;
        }
    }
}
