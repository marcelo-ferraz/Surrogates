using NUnit.Framework;
using System;

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
                .Calls(s => (Func<string>)s.GetDomainName).InOtherDomain())
            // is required to save, as without the file, I would have to figure it out on how to save the assemblybuider to a byte[]
            .Save();

            var simple = new Simple();
            var proxy = Container.Invoke<Simple>();

            Assert.AreNotEqual(simple.GetDomainName(), proxy.GetDomainName());
        }
    }
}