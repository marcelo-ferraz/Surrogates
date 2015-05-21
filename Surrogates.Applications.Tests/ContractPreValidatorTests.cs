using NUnit.Framework;
using Surrogates.Applications.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    public class ContractPreValidatorTests: AppTests
    {
        [Test, ExpectedException(typeof(ArgumentException))]
        public void RequiredTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts(s => (Action<string>)s.Set, These.Are.Required("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set(null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void HasTobeNullTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts(s => (Action<string>)s.Set, These.Are.Null("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set("some value");
        }

        [Test]
        public void EmailTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts(s => (Action<string>)s.Set, These.Are.Email("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set("some@value.com");
        }

        [Test]
        public void UrlTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts(s => (Action<string>)s.Set, These.Are.Url("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set("http://www.google.com");
        }
    }
}
