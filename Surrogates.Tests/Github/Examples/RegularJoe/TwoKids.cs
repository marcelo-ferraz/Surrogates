using System;

namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    public class TwoKids : Kids
    {
        public TwoKids()
        {
            Quantity = 2;
        }

        public void NewMethod(Delegate s_method, int i)
        {
            s_method.DynamicInvoke(i);
        }
    }
}