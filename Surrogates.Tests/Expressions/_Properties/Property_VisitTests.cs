using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using Surrogates.Utilities;
using Surrogates.Utilities.SDILReader;
using System;

namespace Surrogates.Tests.Expressions._Properties
{
    [TestFixture]
    public class Property_VisitTests
    {
        [Test]
        public void WithRegularPropertyAccessors()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                {
                    Set4Property.OneSimpleGetter(a);
                    Set4Property.OneSimpleSetter(a);
                }));

            var proxy =
                container.Invoke<Dummy>();
            
            proxy.AccessItWillThrowException = 2;
            int res = proxy.AccessItWillThrowException; 

            Assert.AreEqual(2, proxy.AccessItWillThrowException);
        }

        [Test]
        public void OnlyGetter_SetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>(d => (Func<Dummy, string, int>) d.SetPropText_InstanceAndMethodName_Return2)));

            var proxy =
                container.Invoke<Dummy>();

            IgnoreException(
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
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>(d => (Func<int, Dummy, int>) d.SetPropText_info_Return_FieldPlus1)))
                //.Save()
                ;

            var proxy =
                container.Invoke<Dummy>();

            IgnoreException(
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
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>(d => (Action<Dummy, int>) d.SetPropText_TypeName)));                              

            var proxy =
                container.Invoke<Dummy>();
            
            IgnoreException(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });
        }

        [Test]
        public void WithFieldAndInstance()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>(d => (Func<int, Dummy, int>) d.SetPropText_info_Return_FieldPlus1))
                );

            var proxy =
                container.Invoke<Dummy>();

            IgnoreException(
                () => proxy.AccessItWillThrowException = 2, 
                () => { int res = proxy.AccessItWillThrowException; });

            Assert.IsTrue(proxy.Text.Contains("was added"));
        }

        public void IgnoreException(params Action[] actions)
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


    public class DummyProxy2 : Dummy
    {
        private InterferenceObject _interceptor = new InterferenceObject();

        private int _accessItWillThrowException;

        private dynamic _stateBag;

        private SurrogatesContainer _container;

        public override int AccessItWillThrowException
        {
            get
            {
                this._interceptor.SetPropText_TypeName(this, _accessItWillThrowException);
                return _accessItWillThrowException;
            }
            set
            {
                this._accessItWillThrowException = value;
            }
        }

        public SurrogatesContainer Container
        {
            get
            {
                return this._container;
            }
            set
            {
                this._container = value;
            }
        }

        public dynamic StateBag
        {
            get
            {
                return this._stateBag;
            }
            set
            {
                this._stateBag = value;
            }
        }

        public DummyProxy2()
        {
            this._interceptor = new InterferenceObject();
            this.StateBag = new object();
            this.Container = new SurrogatesContainer();            
        }

        public DummyProxy2(string str)
            : base(str)
        {
            this._interceptor = new InterferenceObject();
            this.StateBag = new object();
            this.Container = new SurrogatesContainer();            
        }
    }
}