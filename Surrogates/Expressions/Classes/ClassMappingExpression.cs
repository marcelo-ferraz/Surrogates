using Surrogates.Expressions.Methods;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
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

        public InterferenceExpression<T> Replace
        {
            get { return new InterferenceExpression<T>(this, State, InterferenceKind.Substitution); }
        }

        public InterferenceExpression<T> Visit
        {
            get { return new InterferenceExpression<T>(this, State, InterferenceKind.Visitation); }
        }

        internal ClassMappingExpression(string name, MappingState state)
        {
            if (string.IsNullOrEmpty(name))
            { name = DefaultMapper.CreateName4<T>(); }

            State = state;

            try
            {
                State.TypeBuilder = this.State.ModuleBuilder.DefineType(
                    name, TypeAttributes.Public, typeof(T));
            }
            catch (ArgumentException argEx)
            {
                throw new ProxyAlreadyMadeException(typeof(T), name, argEx);
            }
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