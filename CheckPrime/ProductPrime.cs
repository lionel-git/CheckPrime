using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrime
{
    // Plutot stocker (minIndex, maxIndex) sur LastIndex
    class ProductPrime
    {
        // N/(p1.p7.p17)
        public ulong InvProduct { get; set; }
        // Index of last prime in product, Ex: 17 in the example
        public int IndexLastPrime { get; set; }
    }
}
