
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using System.Reflection.Emit;

namespace Surrogates.Expressions
{
    public class NewExpression
    {
        protected ModuleBuilder ModuleBuilder;
        protected BaseContainer4Surrogacy Container;

        protected Strategies ThosePlans;

        internal protected Strategies Strategies 
        {
            get { return ThosePlans; }
            set { ThosePlans = value; } 
        }

        internal protected string Name { get; set; }

        public NewExpression(ModuleBuilder moduleBuilder, BaseContainer4Surrogacy container)
        {
            this.ModuleBuilder = moduleBuilder;
            this.Container = container;
        }

        /// <summary>
        /// The starting point of any expression, where you specify the baseType, its name and its behaviour globally 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the new type. If nothing is provided, it will be the base type's name + 'Proxy'</param>
        /// <param name="access">What every interceptor will be able to see as a parameter</param>
        /// <param name="excludeAccess">What every interceptor will not be able to see as a parameter</param>
        /// <returns></returns>
        public virtual ExpressionFactory<T> From<T>(string name = "", Access? access = null, Access? excludeAccess = null)
        {
            Name = name;

            var p = access.HasValue ? 
                access.Value : 
                Container.DefaultPermissions;

            if (excludeAccess.HasValue)
            {
                p &= ~excludeAccess.Value;
            }

            Strategies = new Strategies(
                typeof(T), name, ModuleBuilder, p);
            
            return new ExpressionFactory<T>(
                Container,
                new Strategy(Strategies), 
                Strategies);
        }
    }
}