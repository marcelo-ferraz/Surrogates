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
                    a.Setter.Using<InterferenceObject>().ThisMethod<int>(d => d.Int_2_ParameterLess)));

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
                  a.Getter.Using<InterferenceObject>().ThisMethod(d => d.Void_ParameterLess)));

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
                    .Getter.Using<InterferenceObject>().ThisMethod<int, Dummy, int>(d => d.Int_1_ReturnFieldAndInstance))
                );

            var proxy =
                container.Invoke<Dummy>();

            try
            {
                proxy.NewExpectedException = 2;
                Assert.Fail();
            }
            catch { }

            Assert.AreEqual(1, proxy.NewExpectedException);
        }
    }



    public class DummyProxy : Dummy
    {
        private InterferenceObject _interference_0;

        private int _newExpectedException;

        public override int NewExpectedException
        {
            get
            {
                this._interference_0.Void_InstanceAndField(this, 0);
                return base.NewExpectedException;
            }
            set
            {
                this._interference_0.Void_InstanceAndField(this, _newExpectedException);
                base.NewExpectedException = value;
            }
        }

        public DummyProxy()
        {
            this._interference_0 = new InterferenceObject();
        }
    }
}