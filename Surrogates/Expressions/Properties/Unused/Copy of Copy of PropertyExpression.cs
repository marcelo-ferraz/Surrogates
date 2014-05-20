using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties.Unused
{
    public class PropertyReplaceExpression<TBase, TSubstitutor> 
        : EndExpression<TBase, TSubstitutor>
    {
        protected PropertyAccessor Accessor;

        internal PropertyReplaceExpression(PropertyAccessor kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            Accessor = kind;
        }

        protected override void RegisterAction(Func<TSubstitutor, Delegate> action)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterFunction(Func<TSubstitutor, Delegate> function)
        {
            throw new NotImplementedException();
        }
    }

    public class PropertyVisitExpression<TBase, TSubstitutor>
        : EndExpression<TBase, TSubstitutor>
    {
        protected PropertyAccessor Accessor;

        internal PropertyVisitExpression(PropertyAccessor kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            Accessor = kind;
        }

        protected override void RegisterAction(Func<TSubstitutor, Delegate> action)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterFunction(Func<TSubstitutor, Delegate> function)
        {
            throw new NotImplementedException();
        }
    }

    //public class PropertyExpression<TBase, T> 
    //    : FluentExpression<AndExpression<TBase>, TBase, TBase>
    //{
    //    protected TypeBuilder Typebuilder;

    //    internal PropertyExpression(IMappingExpression<TBase> mapper, MappingState state)
    //        : base(mapper, state) { }

    //    protected override void RegisterAction(Func<TBase, Delegate> action)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override void RegisterFunction(Func<TBase, Delegate> function)
    //    {
    //        var func = 
    //            function(NotInitializedInstance)
    //            .Method;

    //        for (int i = 0; i < State.Properties.Count; i++)
    //        {
    //            var pName = 
    //                State.Properties[i].Name;
    //            var pType = 
    //                State.Properties[i].PropertyType;

    //            var pField = 
    //                GetField4<T>();

    //            MethodBuilder pGet = Typebuilder.DefineMethod(
    //                string.Concat("get_", pName), 
    //                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, 
    //                pType, 
    //                Type.EmptyTypes);

    //            ILGenerator pILGet = pGet.GetILGenerator();

    //            pILGet.Emit(OpCodes.Ldarg_0);
    //            pILGet.Emit(OpCodes.Ldfld, pField);
    //            pILGet.Emit(OpCodes.Call, func);
    //            pILGet.Emit(OpCodes.Ret);

    //            //The proxy object
    //        //    gen.Emit(OpCodes.Ldarg_0);
    //        //    //The ObjectId to look for
    //        //    gen.Emit(OpCodes.Ldfld, f);
    //        //    gen.Emit(OpCodes.Callvirt, typeof(MongoDatabase).GetMethod("Find", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ObjectId) }, null).MakeGenericMethod(info.PropertyType));
    //        //    gen.Emit(OpCodes.Ret);


    //        //}


    //        //PropertyInfo info = typeof(TBase).GetProperty("Property1", BindingFlags.Public | BindingFlags.Instance);
    //        //{
                
    //        //    MethodBuilder pSet = typeBuilder.DefineMethod("set_" + info.Name, MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { info.PropertyType });
    //        //    ILGenerator pILSet = pSet.GetILGenerator();
    //        //    pILSet.Emit(OpCodes.Ldarg_0);
    //        //    pILSet.Emit(OpCodes.Ldarg_1);
    //        //    pILSet.Emit(OpCodes.Ldarg_0);
    //        //    pILSet.Emit(OpCodes.Ldfld, database);
    //        //    pILSet.Emit(OpCodes.Call, typeof(ProxyBuilder).GetMethod("SetValueHelper", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object), typeof(MongoDatabase) }, null));
    //        //    pILSet.Emit(OpCodes.Stfld, f);
    //        //    pILSet.Emit(OpCodes.Ret);

    //        //    //Edit:  Added fix
    //        //    newProp.SetSetMethod(pSet);
    //        //    newProp.SetGetMethod(setter);
    //        }



    //    }

    //    public PropertyExpression<TBase, T> Property(Func<TBase, T> getProp)
    //    {
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Getter<TInterference>(Func<TBase, Func<T>> getter)
    //    {
    //        RegisterFunction(getter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Getter<TInterferer>(Func<TBase, Func<T, TBase>> getter)
    //    {
    //        RegisterFunction(getter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Getter<TInterferer>(Func<TBase, Func<T, string>> getter)
    //    {
    //        RegisterFunction(getter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Getter<TInterferer>(Func<TBase, Func<T, TBase, string>> getter)
    //    {
    //        RegisterFunction(getter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Setter<TInterferer>(Func<TBase, Action<T>> setter)
    //    {
    //        RegisterFunction(setter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Setter<TInterferer>(Func<TBase, Action<T, TBase>> setter)
    //    {
    //        RegisterFunction(setter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Setter<TInterferer>(Func<TBase, Action<T, string>> setter)
    //    {
    //        RegisterFunction(setter);
    //        return this;
    //    }

    //    public PropertyExpression<TBase, T> Setter<TInterferer>(Func<TBase, Action<T, TBase, string>> setter)
    //    {
    //        RegisterFunction(setter);
    //        return this;
    //    }
    //}
}
