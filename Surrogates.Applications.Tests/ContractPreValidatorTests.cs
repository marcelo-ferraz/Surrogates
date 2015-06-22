using NUnit.Framework;
using Surrogates.Applications.Contracts;
using System;

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
                .Contracts(s => (Action<string>)s.Set, Presume.IsNotNullOrDefault("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set(null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void HasTobeNullTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts(s => (Action<string>)s.Set, Presume.IsNullOrDefault("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set("some value");
        }

        [Test]
        public void EmailTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts(s => (Action<string>)s.Set, Presume.IsAnEmail("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set("some@value.com");
        }

        [Test]
        public void UrlTest()
        {
            Container.Map(m =>
                m.From<Simple>()
                .Apply
                .Contracts("Set", Presume.IsAnUrl("text")));

            var proxy = Container.Invoke<Simple>();

            proxy.Set("http://www.google.com");
        }

        [Test]
        public void CompositeTestRight()
        {
            Container.Map(m =>
                  m.From<Simple>()
                  .Apply
                  .Contracts(s => (Action<string>)s.Set, Presume.That(new Func<string, bool>((string text) => string.IsNullOrEmpty(text))))
            );
            
            var proxy = Container.Invoke<Simple>();

            proxy.Set(null);            
        }
        
        [Test, ExpectedException(typeof(ArgumentException))]
        public void CompositeTestWrong()
        {
            Container.Map(m =>
                  m.From<Simple>()
                  .Apply
                  .Contracts(s => (Action<string>)s.Set, Presume.That(new Func<string, bool>((string text) => !string.IsNullOrEmpty(text))))
            );

            var proxy = Container.Invoke<Simple>();

            proxy.Set(null);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ContainsTest()
        {
            Container.Map(m =>
                  m.From<Simple>()
                  .Apply
                  .Contracts(s => (Action<string>)s.Set, Presume.Contains("t", "text"))
            );
            
            var proxy = Container.Invoke<Simple>();

            proxy.Set("SuperMeatBallBoy");            
        }
    }
}
