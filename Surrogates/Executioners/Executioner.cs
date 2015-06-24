using Surrogates.Model.Collections;
using Surrogates.Tactics;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Executioners
{
    public abstract class Executioner
    {
        public abstract void Execute4Properties(Strategy.ForProperties st);
        public abstract void Execute4Methods(Strategy.ForMethods st);

        protected static FieldInfo GetField(Strategy.InterceptorInfo @int, FieldList fields)
        {            
            return fields.Get(@int.DeclaredType, @int.Name);
        }

        protected static MethodBuilder CreateGetter(Strategy.ForProperties strategy, PropertyInfo prop)
        {
            return strategy.TypeBuilder.DefineMethod(
                string.Concat("get_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.PropertyType,
                Type.EmptyTypes);
        }

        protected static MethodBuilder CreateSetter(Strategy.ForProperties strategy, PropertyInfo prop)
        {
            return strategy.TypeBuilder.DefineMethod(
                string.Concat("set_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new Type[] { prop.PropertyType });
        }

        public void Execute(Strategy st)
        {
            if (st is Strategy.ForProperties)
            { Execute4Properties((Strategy.ForProperties)st); }
            else
            { Execute4Methods((Strategy.ForMethods)st); }
        }
    }
}
