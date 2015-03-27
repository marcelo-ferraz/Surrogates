
using Surrogates.Tests.Simple.Entities;
using System;
namespace Surrogates.Tests.Simple.Methods
{
    public interface IInterferenceTest
    {
        void  BothParameterLess();
        void  PassingBaseParameters();
        void  NotPassingBaseParameters();
        void PassingInstanceAndMethodName(); 
    }

    public class DummyProxy22 : Dummy
    {
        private InterferenceObject _interceptor;

        public DummyProxy22()
        {
            this._interceptor = new InterferenceObject();
        }

        public override void SetPropText_complex(string str, DateTime dateTime, Dummy.EvenMore evenMore)
        {
            _interceptor.Void_VariousParametersWithDifferentNames(null, null, new DateTime(), null, null);
            SetPropText_complex(null, new DateTime(), null);            
        }
    }
}
