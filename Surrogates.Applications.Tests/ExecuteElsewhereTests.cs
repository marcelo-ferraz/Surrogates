using NUnit.Framework;
using Surrogates.Applications.ExecutingElsewhere;
using Surrogates.Applications.Tests;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class ExecuteElsewhereTests : AppTests
    {
        [Test]
        public void SimpleTestInAnotherDomain()
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

        [Test]
        public void SimpleTestInAnotherThread()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Calls(s => (Func<string>)s.GetThreadName).InOtherThread()).Save();

            var simple = new Simple();
            var proxy = Container.Invoke<Simple>();

            Assert.AreNotEqual(simple.GetThreadName(), proxy.GetThreadName());
        }
    }
}