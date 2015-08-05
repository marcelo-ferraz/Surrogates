using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Aspects.Tests
{
    public abstract class AppTestsBase
    {
        protected SurrogatesContainer Container;

        [SetUp]
        public void SetUp()
        {
            Container = new SurrogatesContainer();
        }
    }
}
