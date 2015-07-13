
using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Surrogates.Applications.LazyLoading;
namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class LazyLoadingTests : AppTestsBase
    {
        private const string TEXT = "This text was loaded by this test's loader.";

        private bool _wasLoaded = false;
        public string Load(string propName, Simpleton instance)
        {
            _wasLoaded = true;
            return TEXT;
        }

        [Test]
        public void SimpleLazyLoadingTest()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .LazyLoading(s => s.Text, loader: Load));

            var proxy =
                Container.Invoke<Simpleton>();

            // Validate the lazyloading feature

            Assert.AreEqual(TEXT, proxy.Text);
            Assert.IsTrue(_wasLoaded);

            Assert.IsNullOrEmpty(proxy.Text2);

            // validate the interceptor

            var holder = proxy as IContainsLazyLoadings;
            
            Assert.IsNotNull(holder);
            
            var intProperties = holder
                .LazyLoadingInterceptor
                .Properties;

            var textInterceptor = intProperties
                .Where(i => i.Key == "Text")
                .Select(i => i.Value)
                .First();

            Assert.IsFalse(textInterceptor.IsDirty);
            Assert.AreEqual(TEXT, textInterceptor.Value);

            proxy.Text = "New value";

            Assert.IsTrue(textInterceptor.IsDirty);
            Assert.AreNotEqual(TEXT, textInterceptor.Value);
        }
    }
}