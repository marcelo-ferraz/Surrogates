using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class ExecuteElsewhereTests : AppTests   
    {
        [Test]
        public void SimpleAnotherDomainTest()
        { 
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Calls(s => (Func<string>) s.GetDomainName).InOtherDomain()).Save();

            var simple = new Simple();
            var proxy = Container.Invoke<Simple>();

            Assert.AreNotEqual(simple.GetDomainName(), proxy.GetDomainName());
        }
    }
}
