using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;
using Surrogates.Utils;

namespace Surrogates.Tests.Simple._Properties
{
    [TestFixture]
    public class Property_ReplaceTests
    {
        [Test]
        public void WithRegularPropertyAccessors()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                {
                    With.OneSimpleGetter(a);
                    With.OneSimpleSetter(a);
                }));

            var proxy =
                container.Invoke<Dummy>();

            proxy.AccessItWillThrowException = 1;
            Assert.AreEqual(1, proxy.AccessItWillThrowException);
        }

        [Test]
        public void OnlyGetter_SetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>().ThisMethod<int>(d => d.AccomplishNothing_Return2)));

            var proxy =
                container.Invoke<Dummy>();

            try
            {
                proxy.AccessItWillThrowException = 1;
                Assert.Fail();
            }
            catch { }

            Assert.AreEqual(2, proxy.AccessItWillThrowException);
        }

        [Test]
        public void OnlySetter_GetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>().ThisMethod<int>(d => d.AccomplishNothing_Return2)));

            var proxy =
                container.Invoke<Dummy>();

            proxy.AccessItWillThrowException = 1;

            try
            {
                var val = proxy.AccessItWillThrowException;
                Assert.Fail();
            }
            catch { }
        }

        [Test]
        public void WithVoid()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                  a.Getter.Using<InterferenceObject>().ThisMethod(d => d.AccomplishNothing)));

            var proxy =
                container.Invoke<Dummy>();

            Assert.AreEqual(0, proxy.AccessItWillThrowException);
        }

        [Test]
        public void WithFieldAndInstance()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.SetPropText_info_Return_FieldPlus1))
                );

            var proxy =
                container.Invoke<Dummy>();

            try
            {
                proxy.AccessItWillThrowException = 2;
                Assert.Fail();
            }
            catch { }

            Assert.AreEqual(1, proxy.AccessItWillThrowException);
        }
    }
}