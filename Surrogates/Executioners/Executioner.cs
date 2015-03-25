﻿using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Mappers.Collections;

namespace Surrogates.Executioners
{
    public abstract class Executioner
    {
        public abstract void Execute4Properties(Strategy.ForProperties st);
        public abstract void Execute4Methods(Strategy.ForMethods st);

        private static FieldInfo _lastPrivateField;

        protected static FieldInfo GetField(Strategy.Interceptor @int, FieldList fields)
        {
            if (_lastPrivateField != null &&
                _lastPrivateField.FieldType == @int.DeclaredType)
            {
                return _lastPrivateField;
            }

            return _lastPrivateField =
                fields.Get(@int.DeclaredType, @int.Name);
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
