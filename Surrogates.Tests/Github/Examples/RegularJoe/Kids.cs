
using System;
namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    public class Kids
    {
        public int Quantity { get; set; }

        
        public int MakeTheMath(Func<int> m_get_Age) // <-- this was got throught the all methods search m_ + the exactly name 
        {
            return m_get_Age() + 10 * Quantity;
        }
    }
}