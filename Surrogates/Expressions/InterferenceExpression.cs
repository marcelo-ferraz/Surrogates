using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Expressions.Properties;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using Surrogates.SDILReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Expressions
{
    public class InterferenceExpression<TBase>
         : FluentExpression<MethodInterferenceExpression<TBase>, TBase, TBase>
    {
        private InterferenceKind _kind;

        internal InterferenceExpression(IMappingExpression<TBase> mapper, MappingState state, InterferenceKind kind)
            : base(mapper, state)
        {
            _kind = kind;
        }

        protected override MethodInterferenceExpression<TBase> Return()
        {
            return new MethodInterferenceExpression<TBase>(Mapper, State, _kind);
        }

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
            }

            if (string.IsNullOrEmpty(propName))
            { throw new ArgumentException("What was provided is not a call for an property"); }

            State.Properties.Add(
                typeof(TBase).GetProperty(propName));

            return new AccessorExpression<TBase>(_kind, Mapper, State);
        }
    }
}