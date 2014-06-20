using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests.Github.Examples.LazyLoadIng
{
    class MockedRepository
    {
        internal T Get<T>(string name)
        {
            throw new NotImplementedException(name);
        }
    }
}
