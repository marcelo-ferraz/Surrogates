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
    public class Property_VisitTests
    {
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void WithRegularPropertyAccessors()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                {
                    With.OneSimpleGetter(a);
                    With.OneSimpleSetter(a);
                }));
        }

        [Test]
        public void OnlyGetter_SetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>().ThisMethod<Dummy, string, int>(d => d.Int_2_InstanceAndMethodName)))
                    ;

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.NewExpectedException = 2,
                () => { int res = proxy.NewExpectedException; });

            Assert.IsTrue(proxy.Text.Contains("Dummy"));
        }

        [Test]
        public void OnlySetter_GetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.Int_1_ReturnFieldAndInstance)))
                    .Save();

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.NewExpectedException = 2,
                () => { int res = proxy.NewExpectedException; });

            Assert.IsTrue(proxy.Text.Contains("was added"));
        }

        [Test]
        public void WithVoid()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a =>
                  a.Getter.Using<InterferenceObject>().ThisMethod<Dummy, int>(d => d.Void_InstanceAndField)))
                  ;

            var proxy =
                container.Invoke<Dummy>();
            
            Except(
                () => proxy.NewExpectedException = 2,
                () => { int res = proxy.NewExpectedException; });

            Assert.IsTrue(proxy.Text.Contains("Dummy"));
        }

        [Test]
        public void WithFieldAndInstance()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisProperty(d => d.NewExpectedException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.Int_1_ReturnFieldAndInstance))
                );

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.NewExpectedException = 2, 
                () => { int res = proxy.NewExpectedException; });

            Assert.IsTrue(proxy.Text.Contains("was added"));
        }

        public void Except(params Action[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {

                try
                {
                    actions[i]();
                    Assert.Fail();
                }
                catch { }
            }
        }
    }
}