using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrime
{
    public class MathPrime
    {
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
            //Console.WriteLine($"L={b_.Length}");

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

        public static long Count(ulong N)
        {
            var sqrtN = (ulong)Math.Sqrt(N);
            if (!(sqrtN * sqrtN <= N && N < (sqrtN + 1) * (sqrtN + 1)))
                throw new Exception("Invalid square root!!"); // Check float approx
            var primes = GeneratePrimes(sqrtN);

            var Ndiv2 = N / 2 + 1;

            var productPrimes = new List<ProductPrime>();
            productPrimes.Add(new ProductPrime() { Product = 1, IndexLastPrime = -1 });
            var newProductPrimes = new List<ProductPrime>();
            
            long totalSum = 0;
            long sign = +1;
            do
            {
                ulong sum = 0;
                foreach (var productPrime in productPrimes)
                {
                    int primeIndex = productPrime.IndexLastPrime + 1;
                    while (primeIndex < primes.Count)
                    {
                        ulong newProduct = productPrime.Product * primes[primeIndex];                        
                        if (newProduct <= N)
                        {
                            if (newProduct >= Ndiv2)
                                sum += 1;                            
                            else
                                sum += N / newProduct;
                            if (newProduct * primes[primeIndex] <= N)
                                newProductPrimes.Add(new ProductPrime() { Product = newProduct, IndexLastPrime = primeIndex });
                            primeIndex++;
                        }
                        else
                            primeIndex = primes.Count;
                    }
                }

                totalSum += sign * (long)sum;
                sign = -sign;

                // Exchange pointers
                var tmp = productPrimes;
                productPrimes = newProductPrimes;
                newProductPrimes = tmp;
                newProductPrimes.Clear();
            }
            while (productPrimes.Count > 0);
            return (long)(N - 1) - totalSum + primes.Count;
        }
    }
}
