using Surrogates.Model.Entities;
using Surrogates.Tactics;
using System;
using System.Linq;

namespace Surrogates.Expressions
{
    public class ExpressionFactory<TBase> : Expression<TBase, Strategy>
    {
        internal ExpressionFactory(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies)
        { }

        /// <summary>
        /// Starts an expression that will replace methods's or properties's behaviours
        /// </summary>
        public ReplaceExpression<TBase> Replace
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Replace;
                return new ReplaceExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        /// <summary>
        /// Starts an expression that will disable methods's or properties's behaviours
        /// </summary>
        public DisableExpression<TBase> Disable
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Disable;
                return new DisableExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        /// <summary>
        /// Starts an expression that will visit methods's or properties's behaviours
        /// </summary>
        public VisitExpression<TBase> Visit
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Visit;
                return new VisitExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        /// <summary>
        /// Starts the entry point for extensions to the expression's framework
        /// </summary>
        public ApplyExpression<TBase> Apply
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Extensions;
                return new ApplyExpression<TBase>(Container, Strategies, this);
            }
        }

        /// <summary>
        /// Adds a new property to the new proxy class
        /// </summary>
        /// <param name="type">The type of the new property</param>
        /// <param name="name">The name of the new property</param>
        /// <param name="defaultValue">An default value to be applied to that new property</param>
        /// <returns></returns>
        public AndExpression<TBase> AddProperty(Type type, string name, object defaultValue = null)
        {
            if (defaultValue != null && !defaultValue.GetType().IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    string.Format("The value inside the defaul value is not compatible with the type: '{0}'", type.Name));
            }

            var prop = Strategies
                .NewProperties
                .FirstOrDefault(p => p.Name == name) ;

            if (prop != null)
            {
                prop.Type = type;
                prop.DefaultValue = defaultValue;
            }
            else
            {
                Strategies.NewProperties.Add(
                    new NewProperty(this.Strategies.Builder)
                    {
                        Type = type,
                        Name = name,
                        DefaultValue = defaultValue,
                    });
            }

            return new AndExpression<TBase>(
                this.Container, this.CurrentStrategy, this.Strategies);
        }

        /// <summary>
        /// Adds a new property to the new proxy class
        /// </summary>
        /// <typeparam name="T">The type of the new property</typeparam>
        /// <param name="name">The name of the new property</param>
        /// <param name="defaultValue">An default value to be applied to that new property</param>
        /// <returns></returns>
        public AndExpression<TBase> AddProperty<T>(string name, T defaultValue = default(T))
        {
            return AddProperty(typeof(T), name, defaultValue);
        }

        /// <summary>
        /// Adds a custom attribute to the new proxy class, or one of its member
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <param name="targets">The target of that attribute</param>
        /// <param name="args">The arguments of that attribute</param>
        /// <returns></returns>
        public AndExpression<TBase> AddAttribute(Type type, string memberName = null, AttributeTargets targets = AttributeTargets.All, params object[] args)            
        {
            if (type.GetType().IsAssignableFrom(typeof(Attribute)))
            {
                throw new ArgumentException(
                    string.Format("The provided type '{0}' does not inherit from System.Attribute", type.Name));
            }

            Strategies.NewAttributes.Add(
                new NewAttribute
                {
                    MemberName = memberName,
                    Type = type,
                    Targets = targets,
                    Arguments = args
                });

            return new AndExpression<TBase>(
                this.Container, this.CurrentStrategy, this.Strategies);
        }

        /// <summary>
        /// Adds a custom attribute to the new proxy class, or one of its member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberName"></param>
        /// <param name="targets">The target of that attribute</param>
        /// <param name="args">The arguments of that attribute</param>
        /// <returns></returns>
        public AndExpression<TBase> AddAttribute<T>(string memberName = null, AttributeTargets targets = AttributeTargets.All, params object[] args)
            where T: Attribute
        {
            return AddAttribute(typeof(T), memberName, targets, args);
        }
    }
}