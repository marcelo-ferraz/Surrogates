using Surrogates.Executioners;
using Surrogates.Model.Collections;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

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
            }

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

        private Strategies _owner;

        public Strategy(Strategies owner)
        {
            this._owner = owner;
        }

        public Strategy(Strategy @base)
        {
            this._owner = @base._owner;
            this.Kind = @base.Kind;
        }

        public Type BaseType
        {
            get { return _owner.BaseType; }
        }
        
        public TypeBuilder TypeBuilder
        {
            get { return _owner.Builder; }
        }

        public InterferenceKind Kind { get; set; }
        

        public string InterferenceKindExtended { get; set; }

        public FieldList Fields 
        {
            get { return _owner.Fields; }
        }

        public void Apply(Type baseType, ref TypeBuilder builder)
        {
            var executionerName =
                this.Kind != InterferenceKind.Extensions ?
                Enum.GetName(typeof(InterferenceKind), Kind).ToLower() : 
                InterferenceKindExtended;
            

            Executioners[executionerName](this);
        }
    }
}
