
using Surrogates.Tests.Scenarios.Entities;
using System;
namespace Surrogates.Tests.Unit.Methods
{
    public interface IInterferenceTest
    {
        void  BothParameterLess();
        void  PassingBaseParameters();
        void  NotPassingBaseParameters();
        void PassingInstanceAndMethodName(); 
    }
}
