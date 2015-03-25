﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class UsingInterferenceExpression<TBase> : Expression<TBase, Strategy.ForMethods>
    {
        public UsingInterferenceExpression(Strategy.ForMethods current, Strategies strategies)
            : base(current, strategies) { }

        public AndExpression<TBase> Using<T>(Func<T, Delegate> method)
        {
            return Using<T>(null, method);
        }

        public AndExpression<TBase> Using<T>(string name, Func<T, Delegate> method)
        {
            Strategies.Fields.TryAdd<T>(ref name);
            
            CurrentStrategy.Interceptor =
               new Strategy.Interceptor
               {
                   DeclaredType = typeof(T),
                   Name = name,
                   Method = method(GetNotInit<T>()).Method
               };

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(new Strategy(Strategies), Strategies);
        }
    }
}