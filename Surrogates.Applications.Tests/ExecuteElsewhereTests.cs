using NUnit.Framework;
using Surrogates.Aspects.ExecutingElsewhere;
using Surrogates.Aspects.Tests;
using Surrogates.Model.Entities;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surrogates.Aspects.Tests
{
    [TestFixture]
    public class ExecuteElsewhereTests : AppTestsBase
    {
        private static IntPtr _handleGetThreadId;

        static ExecuteElsewhereTests() 
        {
            _handleGetThreadId = typeof(Simpleton)
                .GetMethod("GetThreadIdAndWaitALittle")
                .MethodHandle
                .Value;
        }

        [Test]
        public void SimpleTestInAnotherDomain()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .Calls(s => (Func<string>)s.GetDomainName).InOtherDomain())
            // is required to save, as without the file, I would have to figure it out on how to save the assemblybuider to a byte[]
            .Save();

            var simple = new Simpleton();
            var proxy = Container.Invoke<Simpleton>();

            Assert.AreNotEqual(simple.GetDomainName(), proxy.GetDomainName());
        }

        [Test]
        public void SimpleTestInAnotherThread()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .Calls(s => (Func<string>)s.GetThreadName).InOtherThread());

            var simple = new Simpleton();
            var proxy = Container.Invoke<Simpleton>();

            Assert.AreNotEqual(simple.GetThreadName(), proxy.GetThreadName());
        }

        [Test]
        public void SimpleTestInAnotherThreadAsync()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .Calls(s => (Func<int, int>)s.GetThreadIdAndWaitALittle).InOtherThread(andForget: true))
                .Save();

            var proxy = Container.Invoke<Simpleton>();
            var simple = new Simpleton();
            
            var result = proxy.GetThreadIdAndWaitALittle(2000);
            var thisTask =
                (proxy as IHasTasks).Tasks[_handleGetThreadId];
            
            var simpleResult = 
                simple.GetThreadIdAndWaitALittle(200);

            thisTask.Wait();
            
            Assert.AreEqual(0, result);
            Assert.AreNotEqual(0, thisTask.Result);
            Assert.AreNotEqual(simpleResult, thisTask.Result);
        }
    }



    public class SimpletonProxy2 : Simpleton, IHasTasks, IContainsStateBag
    {
        private static Dictionary<string, Func<object, Delegate>> _baseMethods;

        private ExecuteInOtherThreadInterceptor _interceptor;

        private Dictionary<IntPtr, bool> _forget;

        private Dictionary<IntPtr, Task<object>> _tasks;

        private DynamicObj _stateBag;

        public Dictionary<IntPtr, bool> Forget
        {
            get
            {
                return this._forget;
            }
            set
            {
                this._forget = value;
            }
        }

        public DynamicObj StateBag
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

        public Dictionary<IntPtr, Task<object>> Tasks
        {
            get
            {
                return this._tasks;
            }
            set
            {
                this._tasks = value;
            }
        }

        static SimpletonProxy2()
        {
            SimpletonProxy2._baseMethods = new Dictionary<string, Func<object, Delegate>>()
            {
                { "GetThreadIdAndWaitALittle", Infer.Delegate<Simpleton>("GetThreadIdAndWaitALittle") }
            };
        }

        public SimpletonProxy2()
        {
            this.Forget = new Dictionary<IntPtr, bool>();
            this.Tasks = new Dictionary<IntPtr, Task<object>>();
            this.StateBag = new DynamicObj();
            this._interceptor = new ExecuteInOtherThreadInterceptor();            
        }

        public SimpletonProxy2(List<int> nums)
        {
            this.Forget = new Dictionary<IntPtr, bool>();
            this.Tasks = new Dictionary<IntPtr, Task<object>>();
            this.StateBag = new DynamicObj();
            this._interceptor = new ExecuteInOtherThreadInterceptor();            
        }

        public override int GetThreadIdAndWaitALittle(int timeout)
        {
            Delegate item = SimpletonProxy2._baseMethods["GetThreadIdAndWaitALittle"](this);
            object[] objArray = new object[] { timeout };
            
            object ret = 
                this._interceptor.Execute(item, objArray, this.Forget, this.Tasks);

            return ret == null ? (int) ret : 0;            
        }

        public class ThisDynamic_
        {
            private object _bag;

            private object _holder;

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

            public ThisDynamic_(object bag, object holder, string callerName, string holderName, Delegate caller, object[] arguments)
            {
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