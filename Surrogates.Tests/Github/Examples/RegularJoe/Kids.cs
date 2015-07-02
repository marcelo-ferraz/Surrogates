
using System;
namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    public class Kids
    {
        public int Quantity { get; set; }

        //verify this
        public int MakeTheMath(Func<int> m_get_Age)
        {
            return m_get_Age() + 10 * Quantity;
        }
    }
}