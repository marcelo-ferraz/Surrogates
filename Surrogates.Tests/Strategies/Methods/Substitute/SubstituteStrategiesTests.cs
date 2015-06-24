using Surrogates.Tactics;
using System;

namespace Surrogates.Tests.Strategies.Methods.Substitute
{
    public class SubstituteStrategiesTests: StrategiesTests<Strategy.ForMethods>
    {
        protected T Replace<T, I>(Delegate method, string name, Delegate interceptor)
        {
            Strategy.Kind = InterferenceKind.Replace;
            this.Strategy.Methods.Add(method.Method);
            this.Strategy.Interceptor = new Strategy.InterceptorInfo(name, typeof(I), interceptor.Method);

            return (T)Activator.CreateInstance(Strategies.Apply().Type);
        }
    }
}
