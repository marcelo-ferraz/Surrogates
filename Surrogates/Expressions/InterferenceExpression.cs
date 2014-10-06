using System;
using System.Reflection;
using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers.Entities;
using Surrogates.SDILReader;

namespace Surrogates.Expressions
{
    /// <summary>
    /// The expression responsible to interfere on a type's behaviour
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public class InterferenceExpression<TBase>
        : EndInterferenceExpression<TBase>        
    {
        private InterferenceKind _kind;

        internal InterferenceExpression(IMappingExpression<TBase> mapper, MappingState state, InterferenceKind kind)
            : base(mapper, state, kind)
        {
            _kind = kind;
        }

        protected override EndInterferenceExpression<TBase> Return()
        {
            return new EndInterferenceExpression<TBase>(Mapper, State, _kind);
        }

        /// <summary>
        /// Exposes a given property to be interefered
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <returns></returns>
        public AccessorExpression<TBase> ThisProperty(string propName)
        {
            if (string.IsNullOrEmpty(propName))
            { throw new ArgumentException("What was provided is not a call for an property"); }

            var prop = typeof(TBase)
                .GetProperty(propName);

            if (!prop.GetGetMethod().IsVirtual)
            { throw new NotSupportedException("The property should be marked as virtual!"); }

            State.Properties.Add(prop);

            return new AccessorExpression<TBase>(_kind, Mapper, State);
        }
        
        /// <summary>
        /// Exposes a given property to be interefered
        /// </summary>
        /// <typeparam name="T">The type of that property</typeparam>
        /// <param name="propGetter">The path to the property</param>
        /// <returns></returns>
        public AccessorExpression<TBase> ThisProperty<T>(Func<TBase, T> propGetter)
        {
            var reader =
                new MethodBodyReader(propGetter.Method);

            string propName = null;

            for (int i = 0; i < reader.Instructions.Count; i++)
            {
                var code =
                    reader.Instructions[i].Code.Name;

                if (code != "callvirt" && code != "call")
                { continue; }

                if (!(reader.Instructions[1].Operand is MethodInfo))
                { continue; }

                propName = ((MethodInfo)reader.Instructions[i].Operand).Name;

                if (!propName.Contains("get_") && !propName.Contains("set_"))
                { throw new ArgumentException("What was provided is not an property"); }

                propName = propName
                    .Replace("get_", string.Empty)
                    .Replace("set_", string.Empty);

                break;
            }

            return ThisProperty(propName);
        }
    }
}