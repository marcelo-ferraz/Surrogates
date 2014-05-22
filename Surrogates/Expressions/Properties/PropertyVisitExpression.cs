using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties
{
    public class PropertyVisitExpression<TBase, TVisitor>
        : PropertyInterferenceExpression<TBase, TVisitor>
    {
        internal PropertyVisitExpression(PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(InterferenceKind.Visitation,accessor, mapper, state) { }
        
        private void Register(Func<TVisitor, Delegate> action)
        {
            var method =
                action(NotInitializedInstance)
                .Method;

            var okForSetter = Accessor != PropertyAccessor.Get;
            var okForGetter = Accessor != PropertyAccessor.Set;

            foreach (var prop in State.Properties)
            {
                var newProp = State.TypeBuilder.DefineProperty(
                    prop.Name,
                    prop.Attributes,
                    prop.PropertyType,
                    null);

                if (okForGetter)
                {
                    newProp.SetSetMethod(
                        OverrideGetter(prop, method));
                }

                if (okForSetter)
                {
                    newProp.SetSetMethod(
                        OverrideSetter(prop, method));
                }
            }
        }

        private MethodBuilder OverrideGetter(PropertyInfo property, MethodInfo newMethod)
        {
            MethodBuilder getter = State.TypeBuilder.DefineMethod(
               string.Concat("get_", property.Name),
               MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
               property.PropertyType,
               Type.EmptyTypes);

            ILGenerator gen = getter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(property.ReflectedType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TVisitor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p =>
                {
                    if (p.Name == "propertyName" && p.ParameterType == typeof(string))
                    { 
                        gen.Emit(OpCodes.Ldstr, property.Name);
                        return true;
                    }
                    return false;
                });

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }
            
            gen.Emit(OpCodes.Call, property.GetMethod);
            
            gen.Emit(OpCodes.Ret);

            return getter;
        }

        private MethodBuilder OverrideSetter(PropertyInfo property, MethodInfo newMethod)
        {
            MethodBuilder setter = State.TypeBuilder.DefineMethod(
               string.Concat("set_", property.Name),
               MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
               typeof(void),
               Type.EmptyTypes);

            ILGenerator gen = setter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(property.ReflectedType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TVisitor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p =>
                {
                    if (p.Name == "propertyName" && p.ParameterType == typeof(string))
                    { 
                        gen.Emit(OpCodes.Ldstr, property.Name);
                        return true;
                    }
                    return false;
                });

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Call, property.GetSetMethod());

            gen.Emit(OpCodes.Ret);

            return setter;
        }

        protected override void RegisterAction(Func<TVisitor, Delegate> action)
        {
            Register(action);
        }

        protected override void RegisterFunction(Func<TVisitor, Delegate> function)
        {
            Register(function);
        }
    }
}