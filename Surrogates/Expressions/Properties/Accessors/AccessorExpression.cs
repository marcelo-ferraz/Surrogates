using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using Surrogates.SDILReader;
using Surrogates.Utils;

namespace Surrogates.Expressions.Properties.Accessors
{
    public class AccessorExpression<TBase>
        : Expression<TBase>
    {
        public AccessorExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { Kind = kind; }

        internal InterferenceKind Kind { get; set; }

        protected virtual void EmitDefaultSet(Property prop)
        {
            //insert a basic set
            MethodBuilder setter = State.TypeBuilder.DefineMethod(
                string.Concat("set_", prop.Original.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new[] { prop.Original.PropertyType });

            ILGenerator gen = setter.GetILGenerator();

            var originalSetter =
                prop.Original.GetSetMethod();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, originalSetter);

            if (originalSetter.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Ret);

            prop.Builder.SetSetMethod(setter);
        }

        protected virtual void EmitBaseGetter(Property prop)
        {
            MethodBuilder getter = State.TypeBuilder.DefineMethod(
                string.Concat("get_", prop.Original.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.Original.PropertyType,
                Type.EmptyTypes);

            ILGenerator gen = getter.GetILGenerator();

            var local = 
                gen.DeclareLocal(prop.Original.PropertyType);

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, prop.Original.GetGetMethod());
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, local);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            prop.Builder.SetGetMethod(getter);
        }


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
        /// <typeparam name="T"></typeparam>
        /// <param name="propGetter"></param>
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
        /// <param name="changeAccessors"></param>
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
                    if (Detect.IsAutomatic(prop))
                    { With.OneSimpleGetter(accessor); }
                    else
                    { EmitBaseGetter(prop); }
                }
            }
            //set was not set
            if ((State.Properties.Accessors & PropertyAccessor.Set) != PropertyAccessor.Set)
            {
                foreach (var prop in State.Properties)
                {
                    if (Detect.IsAutomatic(prop))
                    { With.OneSimpleSetter(accessor); }
                    else
                    { EmitDefaultSet(prop); }
                }
            }

            State.Properties.Clear();

            return new AndExpression<TBase>(Mapper);
        }
   }
}