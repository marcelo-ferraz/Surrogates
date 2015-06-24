using Surrogates.Tactics;
using System;

namespace Surrogates.Expressions
{
    public class DisableExpression<TBase>
      : InterferenceExpression<TBase, AndExpression<TBase>>
    {
        public DisableExpression(BaseContainer4Surrogacy container, Strategy currentStrategy, Strategies strategies)
            : base(container, currentStrategy, strategies) { }

        public override AndExpression<TBase> Methods(string[] methodNames, bool onlyViable = true)
        {
            var result = base.Methods(methodNames, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        public override AndExpression<TBase> Methods(Func<System.Reflection.MethodInfo, bool> predicate, bool onlyViable = true)
        {
            var result = base.Methods(predicate, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }
        
        public override AndExpression<TBase> Methods(params string[] methodNames)
        {
            var result = base.Methods(methodNames);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        public override AndExpression<TBase> These(Func<TBase, Delegate>[] methods, bool onlyViable = true)
        {
            var result = base.These(methods, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        public override AndExpression<TBase> These(params Func<TBase, Delegate>[] methods)
        {
            var result = base.These(methods);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        public override AndExpression<TBase> Properties(params string[] propNames)
        {
            var result = base.Properties(propNames);
            Strategies.Add(CurrentStrategy);
            return result;            
        }

        public override AndExpression<TBase> These(params Func<TBase, object>[] props)
        {
            var result = base.These(props);
            Strategies.Add(CurrentStrategy);
            return result;            
        }
    }
}
