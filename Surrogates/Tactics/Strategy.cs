﻿using Surrogates.Executioners;
using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Tactics
{
    public class Strategy
    {
        static Type _typeofInterferenceKind = TypeOf.InterferenceKind;

        public class InterceptorInfo
        {
            public InterceptorInfo(MethodInfo method)
            {                
                this.Method = method;
            }

            public InterceptorInfo(string name, Type declaredType, MethodInfo method)
            {
                this.Name = string.IsNullOrEmpty(name) ? "interceptor" : name;
                this.DeclaredType = declaredType;
                this.Method = method;
            }

            public string Name { get; set; }
            public Type DeclaredType { get; set; }
            public MethodInfo Method { get; set; }
        }

        public class ForProperties : Strategy
        {
            public ForProperties(Strategies parent)
                :base(parent)
            { Properties = new PropertyList(this); }

            public ForProperties(Strategy @base)
                : base(@base)
            { Properties = new PropertyList(this); }

            public PropertyList Properties { get; set; }

            public InterceptorInfo Getter { set; get; }

            public InterceptorInfo Setter { set; get; }

            public void Add(PropertyInfo prop, bool faultIsException = true)
            {
                var getter = 
                    prop.GetGetMethod(true);
                
                if (faultIsException && (getter.IsFinal || (!getter.IsVirtual && !getter.IsAbstract)))
                {
                    string explanation =
                        getter.IsFinal ? "was marked as sealed" :
                        "either not marked as virtual nor as abstract";

                    throw new NotSupportedException(string.Format(
                        "The getter of the supplied property, '{0}', was {1}. Therefore it cannot be overriden",
                        prop.Name,
                        explanation));
                }

                var setter =
                    prop.GetSetMethod(true);

                if (faultIsException && (setter.IsFinal || (!setter.IsVirtual && !setter.IsAbstract)))
                {
                    string explanation =
                        setter.IsFinal ? "was marked as sealed" :
                        "either not marked as virtual nor as abstract";

                    throw new NotSupportedException(string.Format(
                        "The setter of the supplied property, '{0}', was {1}. Therefore it cannot be overriden",
                        prop.Name,
                        explanation));
                }

                Properties.Add(prop);
            }
        }

        public class ForMethods : Strategy
        {            
            public ForMethods(Strategies parent)
                : base(parent) 
            { Methods = new List<MethodInfo>(); }

            public ForMethods(Strategy @base)
                : base(@base) 
            { Methods = new List<MethodInfo>(); }

            public InterceptorInfo Interceptor { get; set; }

            public IList<MethodInfo> Methods { get; set; }

            public void Add(MethodInfo method, bool faultIsException = true)
            {
                if (faultIsException && (method.IsFinal || (!method.IsVirtual && !method.IsAbstract)))
                {
                    string explanation =
                        method.IsFinal ? "was marked as sealed" : 
                        "either not marked as virtual nor as abstract";

                    throw new NotSupportedException(string.Format(
                        "The supplied method, '{0}', was {1}, therefore it cannot be overriden",
                        method.Name,
                        explanation));
                }

                Methods.Add(method);
            }
        }

        static Strategy()
        {
            Executioners =
                new Dictionary<string, Action<Strategy>>
                {
                    { "replace", new ReplaceExecutioner().Execute },
                    { "visit", new VisitationExecutioner().Execute },
                    { "disable", new DisableExecutioner().Execute },
                };
        }

        public static IDictionary<string, Action<Strategy>> Executioners { get; set; }

        private Strategies _parent;

        public Strategy(Strategies owner)
        {
            this._parent = owner;
        }

        public Strategy(Strategy @base)
        {
            this._parent = @base._parent;
            this.Kind = @base.Kind;
        }

        public InterferenceKind Kind { get; set; }

        public string KindExtended { get; set; }

        public Type BaseType
        {
            get { return _parent.BaseType; }
        }
        
        public TypeBuilder TypeBuilder
        {
            get { return _parent.Builder; }
        }

        public List<NewProperty> NewProperties
        {
            get { return _parent.NewProperties; }
        }

        public Access Accesses
        {
            get { return _parent.Accesses; }
        }

        public FieldList Fields 
        {
            get { return _parent.Fields; }
        }

        public BaseMethods BaseMethods
        {
            get { return _parent.BaseMethods; }
        }

        public NewProperty ContainerProperty
        {
            get { return _parent.ContainerProperty; }
        }

        public NewProperty StateBagProperty
        {
            get { return _parent.StateBagProperty; }
        }

        public Type ThisDynamic_Type
        {
            get { return _parent.ThisDynamic_Type; }
        }

        public void Apply(Type baseType, ref TypeBuilder builder)
        {
            var executionerName =
                this.Kind != InterferenceKind.Extensions ?
                Enum.GetName(_typeofInterferenceKind, Kind).ToLower() : 
                KindExtended;

            if (this.BaseType.IsInterface &&
                (this.Kind == InterferenceKind.Visit ||
                this.Kind == InterferenceKind.Disable))
            {
                throw new NotSupportedException(
                    "Using an interface as base, you ought to use replace, or extensions only.");
            }

            Executioners[executionerName](this);
        }
    }
}
