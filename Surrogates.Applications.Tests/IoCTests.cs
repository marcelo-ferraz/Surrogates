using NUnit.Framework;

namespace Surrogates.Applications.Tests
{
    public class IoCTests : AppTestsBase
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
