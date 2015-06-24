using Surrogates.Tactics;
using System;
using System.Reflection;

namespace Surrogates.Expressions
{
    public class DisableExpression<TBase>
      : InterferenceExpression<TBase, AndExpression<TBase>>
    {
        public DisableExpression(BaseContainer4Surrogacy container, Strategy currentStrategy, Strategies strategies)
            : base(container, currentStrategy, strategies) { }

        /// <summary>
        /// Adds methods, by its names, to the collection of expressions
        /// </summary>
        /// <param name="methodNames"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> Methods(string[] methodNames, bool onlyViable = true)
        {
            var result = base.Methods(methodNames, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }
        
        /// <summary>
        /// Adds any method, as long as it satisfies a predicate, to the collection of expressions
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> Methods(Func<MethodInfo, bool> predicate, bool onlyViable = true)
        {
            var result = base.Methods(predicate, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        /// <summary>
        /// Adds a method, to the collection of expressions
        /// </summary>
        /// <param name="method"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> This(Func<TBase, Delegate> method, bool onlyViable = true)
        {
            return base.This(method, onlyViable);
        }

        /// <summary>
        /// Adds methods, to the collection of expressions
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable methods or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> These(Func<TBase, Delegate>[] methods, bool onlyViable = true)
        {
            var result = base.These(methods, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        /// <summary>
        /// Adds properties if they satisfy a given predicate, to the collection of expressions
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> Properties(Func<PropertyInfo, bool> predicate, bool onlyViable = true)
        {
            var result = base.Properties(predicate, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        /// <summary>
        /// Adds properties by their names, to the collection of expressions
        /// </summary>
        /// <param name="propNames"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> Properties(string[] propNames, bool onlyViable = true)
        {
            var result = base.Properties(propNames, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }

        /// <summary>
        /// Adds properties, to the collection of expressions
        /// </summary>
        /// <param name="props"></param>
        /// <param name="onlyViable">Whether it will throw an exception for non-overrideable properties or just ignore them</param>
        /// <returns></returns>
        public override AndExpression<TBase> These(Func<TBase, object>[] props, bool onlyViable = true)
        {
            var result = base.These(props, onlyViable);
            Strategies.Add(CurrentStrategy);
            return result;
        }
    }
}
