﻿using Surrogates.Executioners;
using Surrogates.Expressions;
using Surrogates.Mappers.Collections;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Tactics
{
    public abstract class Strategy
    {
        public class ForProperties : Strategy
        {
            public PropertyList Properties { get; set; }

            public MethodInfo Getter { set; get; }

            public MethodInfo Setter { set; get; }
        }

        public class ForMethods : Strategy
        {
            public MethodInfo Interceptor { get; set; }

            public IList<MethodInfo> Methods { get; set; }
        }

        static Strategy()
        {
            Executioners =
                new Dictionary<InterferenceKind, ExecutionersAct>
                {
                    { InterferenceKind.Substitution, new ReplaceExecutioner().Execute },
                    { InterferenceKind.Visitation, new VisitationExecutioner().Execute },
                    { InterferenceKind.Disable, new DisableExecutioner().Execute }
                };
        }

        public static IDictionary<InterferenceKind, ExecutionersAct> Executioners { get; set; }

        private Strategies _owner;

        public Strategy(Strategies owner)
        {
            this._owner = owner;
        }

        public Type BaseType
        {
            get { return _owner.BaseType; }
        }
        
        public TypeBuilder TypeBuilder
        {
            get { return _owner.Builder; }
        }

        public string Name { get; set; }

        public InterferenceKind Kind { get; set; }
        
        public Type InterceptorType { get; set; }

        public FieldList Fields 
        {
            get { return _owner.Fields; }
        }

        public void Apply(Type baseType, ref TypeBuilder builder)
        {
            Executioners[Kind](this);
        }        
    }
}
