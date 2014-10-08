using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    public class TwoKids : Kids
    {
        public TwoKids()
        {
            Quantity = 2;
        }

        public void NewMethod(Action<int> method, int i)
        {
            method(i);
        }
    }
}