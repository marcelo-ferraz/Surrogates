using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    public class Kids
    {
        public int Quantity { get; set; }

        public int AddTo(int field)
        {
            return field + 10 * Quantity;
        }

        public void Oop(object[] arguments)
        {
 
        }
    }
}