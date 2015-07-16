using NUnit.Framework;
using Surrogates.Applications.ExecutingElsewhere;
using Surrogates.Applications.Tests;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class ExecuteElsewhereTests : AppTestsBase
    {
        [Test]
        public void SimpleTestInAnotherDomain()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .Calls(s => (Func<string>)s.GetDomainName).InOtherDomain())
            // is required to save, as without the file, I would have to figure it out on how to save the assemblybuider to a byte[]
            .Save();

            var simple = new Simpleton();
            var proxy = Container.Invoke<Simpleton>();

            Assert.AreNotEqual(simple.GetDomainName(), proxy.GetDomainName());
        }

        [Test]
        public void SimpleTestInAnotherThread()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .Calls(s => (Func<string>)s.GetThreadName).InOtherThread());

            var simple = new Simpleton();
            var proxy = Container.Invoke<Simpleton>();

            Assert.AreNotEqual(simple.GetThreadName(), proxy.GetThreadName());
        }
    }
}