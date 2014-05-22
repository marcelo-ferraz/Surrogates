using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Tests.Methods
{
    public interface IInterferenceTest
    {
        void  BothParameterLess();
        void  PassingBaseParameters();
        void  NotPassingBaseParameters();
        void PassingInstanceAndMethodName(); 
    }
}
