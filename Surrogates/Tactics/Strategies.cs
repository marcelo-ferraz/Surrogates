using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Tactics
{
    public class Strategies 
    {
        private TypeBuilder _builder;
        private IList<Strategy> _strategies;

        internal Strategies(Type baseType, string name, ModuleBuilder moduleBuilder, Access permissions)
        {            
            if (string.IsNullOrEmpty(name))
            { name = string.Concat(baseType, "Proxy"); }
            
            try
            {
                Builder = !baseType.IsInterface ?
                    moduleBuilder.DefineType(name, TypeAttributes.Public, baseType) :
                    moduleBuilder.DefineType(name, TypeAttributes.Public, typeof(object), new [] { baseType });
            }
            catch (ArgumentException argEx)
            { throw new ProxyAlreadyMadeException(baseType, name, argEx); }

            this.BaseType = baseType;
            
            this._strategies = 
                new List<Strategy>();
            
            this.Fields = 
                new FieldList(this);
            
            this.NewAttributes = 
                new List<NewAttribute>();

            this.NewInterfaces =
                new List<Type>();

            this.BaseMethods =
                new BaseMethods();

            this.NewProperties =
                new List<NewProperty>();

            this.Accesses = permissions;           
        }

        public Access Accesses { get; set; }

        public Type BaseType { get; set; }

        public TypeBuilder Builder { get; set; }
        
        public FieldList Fields { get; set; }
        
        public List<NewAttribute> NewAttributes { get; set; }

        public List<Type> NewInterfaces { get; set; }

        public BaseMethods BaseMethods { get; set; }

        public List<NewProperty> NewProperties { get; set; }

        public Type ThisDynamic_Type { get; set; }

        public NewProperty ContainerProperty { get; set; }

        public NewProperty StateBagProperty { get; set; }

        private void CreateDefaultNewProperties()
        {
            if (this.Accesses.HasFlag(Access.StateBag) && !this.NewProperties.Any(p => p.Name == "StateBag"))
            {
                this.NewInterfaces.Add(typeof(IContainsStateBag));
                this.StateBagProperty = this.AddProperty<DynamicObj>("StateBag"); 
            }

            if (this.Accesses.HasFlag(Access.Container) && !this.NewProperties.Any(p => p.Name == "Container"))
            { this.ContainerProperty = this.AddProperty<SurrogatesContainer>("Container"); }
        }

        private void ApplyAttributes()
        {
            foreach (var attr in NewAttributes)
            {
                bool forAll =
                    attr.Targets.HasFlag(AttributeTargets.All);

                var ctr = attr.Arguments != null && attr.Arguments.Length > 0 ?
                        attr.Type.GetConstructor(attr.Arguments.Select(arg => arg.GetType()).ToArray()) :
                        attr.Type.GetConstructor(Type.EmptyTypes);

                var attrBuilder =
                    new CustomAttributeBuilder(ctr, attr.Arguments);

                if (string.IsNullOrEmpty(attr.MemberName) && (forAll || attr.Targets.HasFlag(AttributeTargets.Class)))
                {
                    Builder.SetCustomAttribute(attrBuilder);
                    continue;
                }

                foreach (var prop in this.NewProperties)
                {
                    var p = prop.GetBuilder();

                    if (p.Name == attr.MemberName && (forAll || attr.Targets.HasFlag(AttributeTargets.Property)))
                    {
                        p.SetCustomAttribute(attrBuilder);
                        continue;
                    }
                }

                for (int i = 0; i < this.Fields.Count; i++)
                {
                    if (this.Fields[i].Name == attr.MemberName && (forAll || attr.Targets.HasFlag(AttributeTargets.Field)))
                    {
                        this.Fields[i].SetCustomAttribute(attrBuilder);
                        continue;
                    }
                }
            }
        }

        internal NewProperty AddProperty<T>(string name, object defaultValue = null)
        {
            return AddProperty(typeof(T), name, defaultValue);
        }

        internal NewProperty AddProperty(Type type, string name, object defaultValue = null)
        {
            var prop = 
                new NewProperty(this)
                {
                    Name = name,
                    Type = type,
                    DefaultValue = defaultValue
                };

            this.NewProperties.Add(prop);
            return prop;
        }

        private PropertyInfo GetProperty(string name, Type holder)
        {
            return holder.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
        }

        public void Add(Strategy strategy)
        {
            _strategies.Add(strategy);
        }
        
        public Entry Apply()
        {
            ThisDynamic_Type = 
                this.Builder.DefineThisDynamic_NestedType(this);

            this.CreateDefaultNewProperties();

            this.ApplyAttributes();

            this.CreateStaticCtor();

            foreach (var strategy in _strategies)
            {
                strategy.Apply(
                    this.BaseType, ref this._builder);
            }            

            this.CreateConstructor();
            
            foreach (var @interface in NewInterfaces)
            { this.Builder.AddInterfaceImplementation(@interface); }


            var newType = 
                this.Builder.CreateType();

            var props = this
                .NewProperties
                .Select(p => new Entry.Prop{ 
                    Info = GetProperty(p.GetBuilder().Name, newType), 
                    Value = p.DefaultValue
                });

            return new Entry { 
                Type = newType,
                Properties = props.ToArray(),
                StateProperty = GetProperty("StateBag", newType),
                ContainerProperty = GetProperty("Container", newType)                
            };
        }
    }
}
