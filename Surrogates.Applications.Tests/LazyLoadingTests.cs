
using NUnit.Framework;
namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class LazyLoadingTests: AppTestsBase
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

            Assert.AreEqual(TEXT, proxy.Text);
            Assert.IsTrue(_wasLoaded);

            Assert.IsNullOrEmpty(proxy.Text2);
        }
    }
}
