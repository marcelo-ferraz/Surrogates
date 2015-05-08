using Surrogates.Executioners;
using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Tactics
{
    public class Strategy
    {
        public class Interceptor
        {
            public Interceptor(string name, Type declaredType, MethodInfo method)
            {
                this.Name = string.IsNullOrEmpty(name) ? "interceptor" : name;
                this.DeclaredType = declaredType;
                this.Method = method;

                Locals = new Dictionary<string, LocalBuilder>();
            }

            public IDictionary<string, LocalBuilder> Locals { get; set; }

            public string Name { get; set; }
            public Type DeclaredType { get; set; }
            public MethodInfo Method { get; set; }
        }

        public class ForProperties : Strategy
        {
            public ForProperties(Strategies owner)
                :base(owner)
            { Properties = new PropertyList(this); }

            public ForProperties(Strategy @base)
                : base(@base)
            { Properties = new PropertyList(this); }

            public PropertyList Properties { get; set; }

            public Interceptor Getter { set; get; }

            public Interceptor Setter { set; get; }
        }

        public class ForMethods : Strategy
        {
            public ForMethods(Strategies owner)
                : base(owner) 
            { Methods = new List<MethodInfo>(); }

            public ForMethods(Strategy @base)
                : base(@base) 
            { Methods = new List<MethodInfo>(); }

            public Interceptor Interceptor { get; set; }

            public IList<MethodInfo> Methods { get; set; }
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
                Enum.GetName(typeof(InterferenceKind), Kind).ToLower() : 
                KindExtended;            

            Executioners[executionerName](this);
        }
    }
}
