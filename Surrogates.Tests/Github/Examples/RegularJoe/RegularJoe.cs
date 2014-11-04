using System;

namespace Surrogates.Tests.Github.Examples.RegularJoe
{
    public class RegularJoe
    {
        public virtual int Age { get; set; }

        public virtual void Calculate(int i)
        {
            throw new NotImplementedException();
        }
    }
}