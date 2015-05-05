
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using System.Reflection.Emit;

namespace Surrogates.Expressions
{
    public class NewExpression
    {
        private ModuleBuilder _moduleBuilder;
        private BaseContainer4Surrogacy _container;
        internal Strategies Strategies { get; set; }
        internal string Name { get; set; }

        public NewExpression(ModuleBuilder moduleBuilder, BaseContainer4Surrogacy container)
        {
            this._moduleBuilder = moduleBuilder;
            this._container = container;
        }

        public ExpressionFactory<T> From<T>(string name = "", Access? permissions = null, Access? excludeAccess = null)
        {
            Name = name;

            var p = permissions.HasValue ? 
                permissions.Value : 
                _container.DefaultPermissions;

            if (excludeAccess.HasValue)
            {
                p &= ~excludeAccess.Value;
            }

            Strategies = new Strategies(
                typeof(T), name, _moduleBuilder, p);
            
            return new ExpressionFactory<T>(
                _container,
                new Strategy(Strategies), 
                Strategies);
        }
    }
}