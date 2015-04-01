
using Surrogates.Tests.Expressions.Entities;
using System;
namespace Surrogates.Tests.Strategies.Methods
{
    public interface IInterferenceTest
    {
        void  BothParameterLess();
        void  PassingBaseParameters();
        void  NotPassingBaseParameters();
        void PassingInstanceAndMethodName(); 
    }
}
