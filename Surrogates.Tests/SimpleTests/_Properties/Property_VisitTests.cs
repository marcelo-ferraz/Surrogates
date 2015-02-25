using System;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;
using Surrogates.Utils;

namespace Surrogates.Tests.Simple._Properties
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
                .ThisProperty(d => d.AccessItWillThrowException)
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
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>().ThisMethod<Dummy, string, int>(d => d.SetPropText_InstanceAndMethodName_Return2)))
                    ;

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });

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
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.SetPropText_info_Return_FieldPlus1)))
                    .Save();

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });

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
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a =>
                  a.Getter.Using<InterferenceObject>().ThisMethod<Dummy, int>(d => d.SetPropText_TypeName)))
                  ;

            var proxy =
                container.Invoke<Dummy>();
            
            Except(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });

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
                .ThisProperty(d => d.AccessItWillThrowException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.SetPropText_info_Return_FieldPlus1))
                );

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.AccessItWillThrowException = 2, 
                () => { int res = proxy.AccessItWillThrowException; });

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