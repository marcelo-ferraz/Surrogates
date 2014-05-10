using Surrogates.Expressions.Methods;
using Surrogates.Mappers;
using System;
using System.Reflection;

namespace Surrogates.Expressions.Classes
{
    public class ClassMappingExpression<T> : IMappingExpression<T>
    {
        protected MappingState State;
        protected Type ConstructedType;

        public MethodDisableExpression<T> Disable
        {
            get { return new MethodDisableExpression<T>(this, State); }
        }

        public MethodInterceptionExpression<T> Substitute
        {
            get { return new MethodInterceptionExpression<T>(this, State, InterceptionKind.Substitution); }
        }

        public MethodInterceptionExpression<T> Visit
        {
            get { return new MethodInterceptionExpression<T>(this, State, InterceptionKind.Visitation); }
        }

        internal ClassMappingExpression(string name, MappingState state)
        {
            if (string.IsNullOrEmpty(name))
            { name = DefaultMapper.CreateName4<T>(); }
            
            State = state;

            State.TypeBuilder = this.State.ModuleBuilder.DefineType(
                name, TypeAttributes.Public, typeof(T));
        }

        public Type Flush()
        {
            if (ConstructedType == null)
            {
                State
                    .TypeBuilder
                    .CreateConstructor4<T>(State.Fields);

                ConstructedType =
                    State.TypeBuilder.CreateType();
            }

            return ConstructedType;
        }
    }
}