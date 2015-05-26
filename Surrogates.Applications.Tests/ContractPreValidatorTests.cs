using NUnit.Framework;
using Surrogates.Applications.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
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

        [Test]
        public void CompositeTest()
        {
            Func<string, bool> validator = text =>
            {
                return !string.IsNullOrEmpty(text) || text.ToLower().Contains("meat");
            };

            Container.Map(m =>
                  m.From<Simple>()
                  .Apply
                  .Contracts(s => (Action<string>)s.Set, These.Are.Composite(validator)));
            
            var proxy = Container.Invoke<Simple>();
            // Check this style (nunit)
            //NUnit.Framework.Assert.
            proxy.Set("FishBallMeat");            
        }
    }
}
