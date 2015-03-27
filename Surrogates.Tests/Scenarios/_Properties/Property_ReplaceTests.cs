using NUnit.Framework;
using Surrogates.Tests.Scenarios.Entities;
using Surrogates.Utilities;
using System;

namespace Surrogates.Tests.Scenarios._Properties
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
                .From<Dummy>()
                .Replace
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                {
                    Set4Property.OneSimpleGetter(a);
                    Set4Property.OneSimpleSetter(a);
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
                .From<Dummy>()
                .Replace
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>(d => (Func<int>) d.AccomplishNothing_Return2))).Save();

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
                .From<Dummy>()
                .Replace
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>(d => (Func<int>) d.AccomplishNothing_Return2)));

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
                .From<Dummy>()
                .Replace
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                  a.Getter.Using<InterferenceObject>(d => (Action) d.AccomplishNothing)));

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
                .From<Dummy>()
                .Replace
                .This(d => d.AccessItWillThrowException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>(d => (Func<int, Dummy, int>) d.SetPropText_info_Return_FieldPlus1))
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