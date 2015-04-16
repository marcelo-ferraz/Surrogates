﻿using Surrogates.Model.Entities;
using Surrogates.Tactics;
using System;

namespace Surrogates.Expressions
{
    public class ExpressionFactory<TBase> : Expression<TBase, Strategy>
    {
        internal ExpressionFactory(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies)
        { }

        public ReplaceExpression<TBase> Replace
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Replace;
                return new ReplaceExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        public DisableExpression<TBase> Disable
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Disable;
                return new DisableExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        public VisitExpression<TBase> Visit
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Visit;
                return new VisitExpression<TBase>(Container, CurrentStrategy, Strategies);
            }
        }

        public ApplyExpression<TBase> Apply
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Extensions;
                return new ApplyExpression<TBase>(Container, Strategies, this);
            }
        }

        public AndExpression<TBase> AddProperty<T>(string name, T defaultValue = default(T))
        {
            Strategies.NewProperties.Add(
                new NewProperty(this.Strategies.Builder)
                {
                    Type = typeof(T),
                    Name = name,
                    DefaultValue = defaultValue,
                });

            return new AndExpression<TBase>(
                this.Container, this.CurrentStrategy, this.Strategies);
        }

        public AndExpression<TBase> AddAttribute<T>(string memberName = null, AttributeTargets targets = AttributeTargets.All, params object[] args)
            where T: Attribute
        {
            Strategies.NewAttributes.Add(
                new NewAttribute { 
                    MemberName = memberName,
                    Type = typeof(T),
                    Targets = targets,
                    Arguments = args
                });

            return new AndExpression<TBase>(
                this.Container, this.CurrentStrategy, this.Strategies);
        }
    }
}