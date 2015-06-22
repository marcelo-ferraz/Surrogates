using NUnit.Framework;

namespace Surrogates.Applications.Tests
{
    public abstract class AppTests
    {
        protected SurrogatesContainer Container;

        [SetUp]
        public void SetUp()
        {
            Container = new SurrogatesContainer();
        }
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
