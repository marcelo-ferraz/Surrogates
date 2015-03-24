using System;
using System.Reflection;
using Surrogates.Mappers.Entities;
using Surrogates.Utilities.SDILReader;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
namespace Surrogates.OldExpressions.Properties.Accessors
{
    /// <summary>
    /// Exposes the accessors to a given property
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    public class AccessorExpression<TBase>
        : Expression<TBase>
    {
        public AccessorExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { Kind = kind; }

        internal InterferenceKind Kind { get; set; }

        /// <summary>
        /// Exposes a given property to be interefered
        /// </summary>
        /// <param name="propName"></param>
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

            return this;
        }

        /// <summary>
        /// Exposes a given property to be interefered
        /// </summary>
        /// <typeparam name="T">The type that property</typeparam>
        /// <param name="propGetter">The path to that property</param>
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

        /// <summary>
        /// Gives access to the accessors of the property
        /// </summary>
        /// <param name="changeAccessors">The expression that will change the way the accessors of that, or those properties, behave</param>
        /// <returns></returns>
        public AndExpression<TBase> Accessors(Action<AccessorChangeExpression<TBase>> changeAccessors)
        {
            var accessor =
                new AccessorChangeExpression<TBase>(Kind, Mapper, State);

            changeAccessors(accessor);

            //get was not set
            if ((State.Properties.Accessors & PropertyAccessor.Get) != PropertyAccessor.Get)
            {
                foreach (var prop in State.Properties)
                {
                    if (Infer.IsAutomatic(prop))
                    { With.OneSimpleGetter(accessor); }
                    else
                    { prop.EmitBaseGetter(this.State); }
                }
            }
            //set was not set
            if ((State.Properties.Accessors & PropertyAccessor.Set) != PropertyAccessor.Set)
            {
                foreach (var prop in State.Properties)
                {
                    if (Infer.IsAutomatic(prop))
                    { With.OneSimpleSetter(accessor); }
                    else
                    { prop.EmitDefaultSet(this.State); }
                }
            }

            State.Properties.Clear();

            return new AndExpression<TBase>(Mapper);
        }
   }
}