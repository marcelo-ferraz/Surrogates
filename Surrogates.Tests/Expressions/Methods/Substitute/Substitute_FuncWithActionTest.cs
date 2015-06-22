using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;

namespace Surrogates.Tests.Expressions.Methods.Substitute
{
    [TestFixture]
    public class Substitute_FuncWithActionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Replace
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>(r => (Action)r.AccomplishNothing))
                ;

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Replace
                .This(d => (Func<string, DateTime, Dummy.EvenMore, int>)d.Call_SetPropText_complex_Return_1)
                .Using<InterferenceObject>("AddToPropText__MethodName"));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            // just to show that the rest of the object behaves as expected
            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple", proxy.Text);

            //and now, the comparison between the two methods
            var dummyRes =
                dummy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            var proxyRes =
                proxy.Call_SetPropText_complex_Return_1("this call was not made by the original property", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("simple, this call was not made by the original property - property: Call_SetPropText_complex_Return_1", proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.From<Dummy>()
                .Replace
                .Method("Call_SetPropText_complex_Return_1")
                .Using<InterferenceObject>(r => (Action<string, Dummy, DateTime, string, Dummy.EvenMore>)r.Void_VariousParametersWithDifferentNames));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
            proxy.Call_SetPropText_complex_Return_1("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Replace
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1)
                .Using<InterferenceObject>(r => (Action<dynamic>) r.SetPropText_InstanceAndMethodName)).Save();

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            var dummyRes =
                dummy.Call_SetPropText_simple_Return_1();
            var proxyRes =
                proxy.Call_SetPropText_simple_Return_1();

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("simple", dummy.Text);

            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual(typeof(Dummy).FullName + "+Call_SetPropText_simple_Return_1", proxy.Text);
            Assert.AreNotEqual(dummyRes, proxyRes);
            Assert.AreEqual(0, proxyRes);
        }
    }
    public class DummyProxy2 : Dummy
    {
        private static Dictionary<string, Func<object, Delegate>> _baseMethods;

        private InterferenceObject _interceptor = new InterferenceObject();

        private SurrogatesContainer _container;

        private dynamic _stateBag;

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

        static DummyProxy2()
        {
            DummyProxy2._baseMethods = new Dictionary<string, Func<object, Delegate>>()
            {
                { "Call_SetPropText_simple_Return_1", Infer.Delegate<Dummy>("Call_SetPropText_simple_Return_1") }
            };
        }

        public DummyProxy2()
        {
        }

        public DummyProxy2(string str)
            : base(str)
        {
        }

        public override int Call_SetPropText_simple_Return_1()
        {
            Delegate item = DummyProxy2._baseMethods["Call_SetPropText_simple_Return_1"](this);
            object[] objArray = new object[0];
            object[] container = new object[] { this.Container, this.StateBag, this, "Call_SetPropText_simple_Return_1", "Surrogates.Tests.Expressions.Entities.Dummy", item, objArray };
            this._interceptor.SetPropText_InstanceAndMethodName(Activator.CreateInstance(Type.GetType("Surrogates.Tests.Expressions.Entities.DummyProxy+ThisDynamic_"), container));
            return 0;
        }

        public class ThisDynamic_
        {
            private BaseContainer4Surrogacy _container;

            private dynamic _bag;

            private dynamic _holder;

            private string _callerName;

            private string _holderName;

            private Delegate _caller;

            private object[] _arguments;

            public object[] Arguments
            {
                get
                {
                    return this._arguments;
                }
                set
                {
                    this._arguments = value;
                }
            }

            public dynamic Bag
            {
                get
                {
                    return this._bag;
                }
                set
                {
                    this._bag = value;
                }
            }

            public Delegate Caller
            {
                get
                {
                    return this._caller;
                }
                set
                {
                    this._caller = value;
                }
            }

            public string CallerName
            {
                get
                {
                    return this._callerName;
                }
                set
                {
                    this._callerName = value;
                }
            }

            public BaseContainer4Surrogacy Container
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

            public dynamic Holder
            {
                get
                {
                    return this._holder;
                }
                set
                {
                    this._holder = value;
                }
            }

            public string HolderName
            {
                get
                {
                    return this._holderName;
                }
                set
                {
                    this._holderName = value;
                }
            }

            public ThisDynamic_(BaseContainer4Surrogacy container, object bag, object holder, string callerName, string holderName, Delegate caller, object[] arguments)
            {
                this.Container = container;
                this.Bag = bag;
                this.Holder = holder;
                this.CallerName = callerName;
                this.HolderName = holderName;
                this.Caller = caller;
                this.Arguments = arguments;
            }
        }
    }
}
