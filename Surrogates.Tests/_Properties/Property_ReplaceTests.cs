using NUnit.Framework;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Tests.Entities;
using Surrogates.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Tests._Properties
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
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                {
                    With.OneSimpleGetter(a);
                    With.OneSimpleSetter(a);
                }));

            var proxy = 
                container.Invoke<Dummy>();

            proxy.NewExpectedException = 1;
            Assert.AreEqual(1, proxy.NewExpectedException);
        }

        [Test]
        public void OnlyGetter_SetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>().ThisMethod<int>(d => d.Int_2_ParameterLess)));

            var proxy =
                container.Invoke<Dummy>();

            try
            {
                proxy.NewExpectedException = 1;
                Assert.Fail();
            }
            catch { }

            Assert.AreEqual(2, proxy.NewExpectedException);
        }

        [Test]
        public void OnlySetter_GetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>().ThisMethod<int>(d => d.Int_2_ParameterLess))).Save();

            var proxy =
                container.Invoke<Dummy>();

            proxy.NewExpectedException = 1;

            try
            {
                var val = proxy.NewExpectedException;
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
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a => 
                    a.Getter.Using<InterferenceObject>().ThisMethod<int>(d => d.Int_2_ParameterLess)));

            var proxy = 
                container.Invoke<Dummy>();

            Assert.AreEqual(0, proxy.NewExpectedException);
         }

        [Test]
        public void WithFieldAndInstance()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Replace
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.Int_ReturnFieldAndInstance))
                ).Save();



        }
    }
}
