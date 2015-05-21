using NUnit.Framework;
using Surrogates.Applications.Interlocking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    public abstract class AppTests
    {
        protected SurrogatesContainer Container = new SurrogatesContainer();
    }

    public class IoCTests : AppTests
    {
        [Test]
        public void SimpleInjection()
        {
            //Container.Map(m =>
            //    m.From<Simple>()
            //    .Apply
            //    .InjectOn(s => s.List).This<List<int>>());
        }
    }
}
