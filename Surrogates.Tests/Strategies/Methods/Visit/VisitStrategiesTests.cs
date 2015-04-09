using Surrogates.Tactics;
using System;

namespace Surrogates.Tests.Strategies.Methods.Visit
{
    public class VisitStrategiesTests: StrategiesTests<Strategy.ForMethods>
    {
        protected T Replace<T, I>(Delegate method, string name, Delegate interceptor)
        {
            Strategy.Kind = InterferenceKind.Visit;
            this.Strategy.Methods.Add(method.Method);
            this.Strategy.Interceptor = new Strategy.Interceptor(name, typeof(I), interceptor.Method);

            return (T)Activator.CreateInstance(Strategies.Apply().Type);
        }
    }
}
