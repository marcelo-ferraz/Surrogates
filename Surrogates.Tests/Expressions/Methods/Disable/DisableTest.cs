using NUnit.Framework;
using Surrogates.Tests.Expressions.Entities;
using System;
using Surrogates.Utilities;
using System.Collections.Generic;

namespace Surrogates.Tests.Expressions.Methods.Disable
{
    [TestFixture]
    public class DisableTest
    {
        [Test]
        public void Void()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Disable
                .Method("SetPropText_simple")).Save();

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.SetPropText_simple();
            proxy.SetPropText_simple();

            Assert.AreEqual("simple", dummy.Text);
            Assert.IsNullOrEmpty(proxy.Text);
        }
        [Test]
        public void WithReturn()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Disable
                .This(d => (Func<int>)d.Call_SetPropText_simple_Return_1));

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

    }

            public class ThisBig_2
        {
            private BaseContainer4Surrogacy _container;

            private dynamic _bag;

            private dynamic _instance;

            private string _methodName;

            private string _className;

            private Delegate _baseMethod;

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

            public Delegate BaseMethod
            {
                get
                {
                    return this._baseMethod;
                }
                set
                {
                    this._baseMethod = value;
                }
            }

            public string ClassName
            {
                get
                {
                    return this._className;
                }
                set
                {
                    this._className = value;
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

            public dynamic Instance
            {
                get
                {
                    return this._instance;
                }
                set
                {
                    this._instance = value;
                }
            }

            public string MethodName
            {
                get
                {
                    return this._methodName;
                }
                set
                {
                    this._methodName = value;
                }
            }

            public ThisBig_2(BaseContainer4Surrogacy container, object bag, object instance, string methodName, string className, Delegate baseMethod, object[] args)
            {
                Container = container;
                Bag = bag;
                Instance = instance;
                MethodName = methodName;
                ClassName = className;
                BaseMethod = baseMethod;
                Arguments = args;

                var obj = new object[] {
                    this.Container, this.Bag, null, "mname", "cname"
                };

                Activator.CreateInstance(typeof(ThisBig_2), obj);
            }
        }



            public class DummyProxy : Dummy
            {
                private dynamic _stateBag;

                private SurrogatesContainer _container;

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

                public DummyProxy()
                {
                    this.StateBag = new object();
                    this.Container = new SurrogatesContainer();
                }

                public DummyProxy(string str)
                    : base(str)
                {
                    this.StateBag = new object();
                    this.Container = new SurrogatesContainer();
                }

                public override int Call_SetPropText_simple_Return_1()
                {                    
                    //var args = new object[] { };

                    var obj = new object [7];
                    //obj[0] = this.Container; 
                    //obj[1] = this.StateBag;
                    //obj[2] = this;
                    //obj[3] = "";
                    //obj[4] = ""; 
                    //obj[5] = null;
                    //obj[6] = args;

                    Nhonho(Activator.CreateInstance(Type.GetType("strategy.ThisDynamic_Type.FullName"), obj));

                    return 0;
                }

                public void Nhonho(dynamic _)
                {

                }

                public class ThisBig_
                {
                    private BaseContainer4Surrogacy _container;

                    private dynamic _bag;

                    private dynamic _instance;

                    private string _methodName;

                    private string _className;

                    private Delegate _baseMethod;

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

                    public Delegate BaseMethod
                    {
                        get
                        {
                            return this._baseMethod;
                        }
                        set
                        {
                            this._baseMethod = value;
                        }
                    }

                    public string ClassName
                    {
                        get
                        {
                            return this._className;
                        }
                        set
                        {
                            this._className = value;
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

                    public dynamic Instance
                    {
                        get
                        {
                            return this._instance;
                        }
                        set
                        {
                            this._instance = value;
                        }
                    }

                    public string MethodName
                    {
                        get
                        {
                            return this._methodName;
                        }
                        set
                        {
                            this._methodName = value;
                        }
                    }

                    public ThisBig_(BaseContainer4Surrogacy container, object bag, object instance, string methodName, string className, Delegate baseMethod, object[] arguments)
                    {
                        this.Container = container;
                        this.Bag = bag;
                        this.Instance = instance;
                        this.MethodName = methodName;
                        this.ClassName = className;
                        this.BaseMethod = baseMethod;
                        this.Arguments = arguments;
                    }
                }
            }



                public class DummyProxy2 : Dummy
                {
                    private dynamic _stateBag;

                    private SurrogatesContainer _container;

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

        private static Dictionary<string, Func<object, Delegate>> _baseMethods;

        private InterferenceObject _interceptor = new InterferenceObject();

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

        public DummyProxy2(string str) : base(str)
        {
        }

        public override int Call_SetPropText_simple_Return_1()
        {
            object[] objArray = new object[0];

            var obj = new object[7];
            obj[0] = this.Container; 
            //obj[1] = this.StateBag;
            //obj[2] = this;
            obj[3] = "";
            //obj[4] = ""; 
            //obj[5] = null;
            obj[6] = objArray;

            _interceptor.SetPropText_InstanceAndMethodName(Activator.CreateInstance(Type.GetType("strategy.ThisDynamic_Type.FullName"), obj));
            return 2;
        }

        public class ThisBig_
        {
            private BaseContainer4Surrogacy _container;

            private dynamic _bag;

            private dynamic _instance;

            private string _methodName;

            private string _className;

            private Delegate _baseMethod;

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

            public Delegate BaseMethod
            {
                get
                {
                    return this._baseMethod;
                }
                set
                {
                    this._baseMethod = value;
                }
            }

            public string ClassName
            {
                get
                {
                    return this._className;
                }
                set
                {
                    this._className = value;
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

            public dynamic Instance
            {
                get
                {
                    return this._instance;
                }
                set
                {
                    this._instance = value;
                }
            }

            public string MethodName
            {
                get
                {
                    return this._methodName;
                }
                set
                {
                    this._methodName = value;
                }
            }

            public ThisBig_(BaseContainer4Surrogacy container, object bag, object instance, string methodName, string className, Delegate baseMethod, object[] arguments)
            {
                this.Container = container;
                this.Bag = bag;
                this.Instance = instance;
                this.MethodName = methodName;
                this.ClassName = className;
                this.BaseMethod = baseMethod;
                this.Arguments = arguments;
            }
        }
    }
}
