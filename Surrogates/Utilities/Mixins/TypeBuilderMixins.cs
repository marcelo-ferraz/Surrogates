using Surrogates.Model.Entities;
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Surrogates.Utilities.Mixins
{
    internal static class TypeBuilderMixins
    {
        private static bool Has(this MethodAttributes attrs, MethodAttributes other)
        {
            return (attrs | other) != other;
        }

        internal static void CreateStaticCtor(this Strategies strats)
        {
            if (strats.BaseMethods.Count < 1) { return; }

            // get the Dictionary type
            var dicType = typeof(Dictionary<,>).MakeGenericType(
                typeof(string),
                typeof(Func<,>).MakeGenericType(typeof(object), typeof(Delegate)));

            // gets the Add method from _dictionary
            var dicAddMethod = dicType.GetMethod("Add");

            // creates the _baseMethods srcField
            var baseMethodsField =
                strats.Builder.DefineField("_baseMethods", dicType, FieldAttributes.Private | FieldAttributes.Static);

            strats.BaseMethods.Field = baseMethodsField;

            // creates the static constructor
            var cctr = strats
                .Builder
                .DefineConstructor(MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

            // gets the Infer.Delegate method
            var getDel = typeof(Infer)
                .GetMethod("Delegate", new[] { typeof(string) })
                .MakeGenericMethod(strats.BaseType);

            var gen = cctr.GetILGenerator();

            //IL_0001: newobj instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [mscorlib]System.Func`2<class Surrogates.Applications.Tests.SimpleProxy, class [mscorlib]System.Delegate>>::.ctor()
            gen.Emit(OpCodes.Newobj, dicType.GetConstructor(Type.EmptyTypes));
            //IL_0006: stsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [mscorlib]System.Func`2<class Surrogates.Applications.Tests.SimpleProxy, class [mscorlib]System.Delegate>> Surrogates.Applications.Tests.SimpleProxy::_baseMethods
            gen.Emit(OpCodes.Stsfld, baseMethodsField);

            foreach (var method in strats.BaseMethods)
            {
                //IL_000b: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [mscorlib]System.Func`2<class Surrogates.Applications.Tests.SimpleProxy, class [mscorlib]System.Delegate>> Surrogates.Applications.Tests.SimpleProxy::_baseMethods
                gen.Emit(OpCodes.Ldsfld, baseMethodsField);
                //IL_0010: ldstr "Add2List"
                gen.Emit(OpCodes.Ldstr, method.Name);
                //IL_0015: ldstr "Add2List"
                gen.Emit(OpCodes.Ldstr, method.Name);
                //IL_001a: call class [mscorlib]System.Func`2<object, class [mscorlib]System.Delegate> [Surrogates]Surrogates.Utilities.Infer::Delegate<class Surrogates.Applications.Tests.Simple>(string)
                gen.Emit(OpCodes.Call, getDel);
                //IL_001f: callvirt instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [mscorlib]System.Func`2<class Surrogates.Applications.Tests.SimpleProxy, class [mscorlib]System.Delegate>>::Add(!0,  !1)
                gen.EmitCall(dicAddMethod);
            }

            //IL_0024: nop
            //IL_0025: ret
            gen.Emit(OpCodes.Ret);
        }

        internal static void CreateConstructor(this Strategies strats)
        {
            Action<Type[], MethodAttributes> define4 =
                (types, attr) =>
                    strats.Builder.DefineConstructor(attr, CallingConventions.Standard, types)
                        .GetILGenerator()
                        .EmitConstructor(strats, types);

            var ctrs = strats.BaseType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                //.Concat(baseType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic));

            bool hasParameterlessCtr = false;

            foreach (var ctr in ctrs)
            {
                var pTypes = 
                    ctr.GetParameters().Select(p => p.ParameterType).ToArray();

                if (pTypes.Length < 1) 
                { hasParameterlessCtr = true; }

                var attrs = ctr.Attributes.Has(MethodAttributes.Public) ? 
                    MethodAttributes.Public:  
                    MethodAttributes.FamANDAssem;

                define4(pTypes, attrs);
            }

            if (!hasParameterlessCtr)
            { define4(Type.EmptyTypes, MethodAttributes.Public); }
        }
             
        internal static FieldBuilder DefineFieldFromProperty(this TypeBuilder builder, PropertyInfo prop)
        {
            var fieldName = prop.Name;

            fieldName = string.Concat(
                '_', Char.ToLower(fieldName[0]), fieldName.Substring(1));

            return builder.DefineField(fieldName, prop.PropertyType, FieldAttributes.Private);
        }

        internal static PropertyBuilder DefineNewProperty<T>(this TypeBuilder builder, string name, FieldInfo field = null)
        {
            return builder.DefineNewProperty(typeof(T), name, field); 
        }

        internal static PropertyBuilder DefineNewProperty(this TypeBuilder builder, Type type, string name, FieldInfo field = null)
        {
            var getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            #region getter

            Func<FieldInfo, MethodBuilder> get_Prop =
                f =>
                {
                    // Define the "get" accessor method for CustomerName.
                    MethodBuilder getterBuilder = builder.DefineMethod(
                        string.Concat("get_", name), getSetAttr, type, Type.EmptyTypes);

                    ILGenerator getIL =
                        getterBuilder.GetILGenerator();

                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, f);

                    if (f.FieldType != type)
                    { getIL.Emit(OpCodes.Castclass, type); }
                    
                    getIL.Emit(OpCodes.Ret);

                    return getterBuilder;
                };

            #endregion

            #region setter

            Func<FieldInfo, MethodBuilder> set_Prop =
                f =>
                {
                    // Define the "set" accessor method for CustomerName.
                    MethodBuilder setterBuilder = builder.DefineMethod(
                        string.Concat("set_", name), getSetAttr, null, new Type[] { type });

                    ILGenerator setIL =
                        setterBuilder.GetILGenerator();

                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    
                    if (f.FieldType != type)
                    { setIL.Emit(OpCodes.Castclass, type); }

                    setIL.Emit(OpCodes.Stfld, f);                    
                    setIL.Emit(OpCodes.Ret);

                    return setterBuilder;
                };

            #endregion

            if (field == null)
            {
                field = builder.DefineField(
                    string.Format("_{0}{1}", name.Substring(0, 1).ToLower(), name.Substring(1)), type, FieldAttributes.Private);
            }

            var propBldr = builder.DefineProperty(
                name, PropertyAttributes.HasDefault, type, null);

            if (type.IsAssignableFrom(typeof(IDynamicMetaObjectProvider)))
            {
                var dynamicAttrCtor = typeof(DynamicAttribute)
                    .GetConstructor(Type.EmptyTypes);

                //srcField.SetCustomAttribute(
                //        new CustomAttributeBuilder(dynamicAttrCtor, new object[] { }));

                propBldr.SetCustomAttribute(
                    new CustomAttributeBuilder(dynamicAttrCtor, new object[] { }));
            }

            propBldr.SetGetMethod(get_Prop(field));
            propBldr.SetSetMethod(set_Prop(field));
            
            return propBldr;
        }

        internal static Type DefineThisDynamic_NestedType(this TypeBuilder self, Strategies strats)
        {
            Func<Access, bool> can = 
                a => strats.Accesses.HasFlag(a);

            var builder =
                self.DefineNestedType("ThisDynamic_", TypeAttributes.Class | TypeAttributes.NestedPublic);

            var props = new List<PropertyBuilder>();

            if (can(Access.Container))
            { props.Add(builder.DefineNewProperty<BaseContainer4Surrogacy>("Container")); }

            if (can(Access.StateBag))
            { props.Add(builder.DefineNewProperty<dynamic>("Bag")); }

            if(can(Access.Instance))
            { props.Add(builder.DefineNewProperty<object>("Holder")); }

            props.Add(builder.DefineNewProperty<string>("CallerName"));
            props.Add(builder.DefineNewProperty<string>("HolderName"));
            props.Add(builder.DefineNewProperty<Delegate>("Caller"));
            props.Add(builder.DefineNewProperty<object[]>("Arguments"));

            var ctor = builder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, props.Select(p => p.PropertyType).ToArray());

            var gen = ctor.GetILGenerator();
            
            int pIndex = 0;
            var @params = props.Select(
                p => ctor.DefineParameter(++pIndex, ParameterAttributes.None, 
                    string.Concat(p.Name[0].ToString().ToLower(), p.Name.Substring(1)))).ToArray(); 
            
            for (int i = 0; i < props.Count; i++)
            {
                var type =
                    props[i].PropertyType;

                gen.Emit(OpCodes.Ldarg_0);

                gen.Emit(OpCodes.Ldarg, (sbyte) i + 1); 
                gen.EmitCall(props[i].GetSetMethod());
            }

            gen.Emit(OpCodes.Ret);

            return builder.CreateType();
        }
    }
}