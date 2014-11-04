using System;

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
