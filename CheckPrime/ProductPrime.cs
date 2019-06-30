using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrime
{
    class ProductPrime
    {
        // Product of some primes, Ex: Product = p1.p4.p8.p17
        public ulong Product { get; set; }
        // Index of last prime in product, Ex: 17 in the example
        public int IndexLastPrime { get; set; }
    }
}
